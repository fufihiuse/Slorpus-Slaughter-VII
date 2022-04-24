using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Slorpus.Utils
{
    /*
     * Custom bloom class, replacement for the apparently nonfunctional
     * BloomFilter class that I stole? idk what was up with that
     */
    class SimpleBloom
    {
        Effect Shader;

        EffectPass Extract;
        EffectPass ExtractLuminance;
        EffectPass Upsample;
        EffectPass Downsample;
        EffectPass UpsampleLuminance;

        EffectParameter InverseResolution;
        EffectParameter Radius;
        EffectParameter Strength;
        EffectParameter StreakLength;
        EffectParameter Threshold;

        Vector2 _IRVector;
        Vector2 IRVector
        {
            get { return _IRVector; }
            set { 
                _IRVector = value;
                InverseResolution.SetValue(value);
            }
        }

        RenderTarget2D extracted;
        RenderTarget2D[] Mips;

        GraphicsDevice GraphicsDevice;

        public SimpleBloom(
            ContentManager content,
            GraphicsDevice graphicsDevice,
            Point size, 
            int passes)
        {
            GraphicsDevice = graphicsDevice;
            Shader = content.Load<Effect>("shaders/bloom");

            Extract = Shader.Techniques["Extract"].Passes[0];
            ExtractLuminance = Shader.Techniques["ExtractLuminance"].Passes[0];
            Upsample = Shader.Techniques["Upsample"].Passes[0];
            Downsample = Shader.Techniques["Downsample"].Passes[0];
            UpsampleLuminance = Shader.Techniques["UpsampleLuminance"].Passes[0];
            
            InverseResolution = Shader.Parameters["InverseResolution"];
            Radius = Shader.Parameters["Radius"];
            Strength = Shader.Parameters["Strength"];
            StreakLength = Shader.Parameters["StreakLength"];
            Threshold = Shader.Parameters["Threshold"];

            // set parameters
            IRVector = Vector2.Divide(Vector2.One, size.ToVector2());
            InverseResolution.SetValue(IRVector); // one over size
            Radius.SetValue(4.0f);
            Strength.SetValue(10.0f);
            StreakLength.SetValue(5.0f);
            Threshold.SetValue(1.0f);

            // generate rendertargets for mipmaps
            Mips = new RenderTarget2D[passes];
            Point targetSize = size;
            for (int i = 0; i < passes; i++)
            {
                Mips[i] = new RenderTarget2D(
                    GraphicsDevice,
                    targetSize.X, targetSize.Y,
                    false,
                    GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24
                    );
                targetSize /= new Point(2,2);
            }

            extracted = new RenderTarget2D(
                GraphicsDevice,
                size.X, size.Y,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24
                );
        }

        public void Apply(SpriteBatch sb, RenderTarget2D input, ref RenderTarget2D output)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
            
            // draw extracted colors
            GraphicsDevice.SetRenderTarget(extracted);
            Extract.Apply();
            sb.Draw(input, extracted.Bounds, Color.White);

            // now draw all the mips with downsampling
            // target which changes with iteration
            RenderTarget2D pong = extracted;
            foreach (RenderTarget2D mip in Mips)
            {
                GraphicsDevice.SetRenderTarget(mip);
                Downsample.Apply();
                sb.Draw(pong, mip.Bounds, Color.White);
                // draw this mip to the next one
                pong = mip;
                IRVector *= 2;
            }

            // compose mips back together into final texture (with linear sampling)
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap);

            for (int i = Mips.Length-1; i >= 0; i--)
            {
                pong = (i >= 1) ? Mips[i - 1] : output;

                GraphicsDevice.SetRenderTarget(pong);

                Upsample.Apply();
                sb.Draw(Mips[i], pong.Bounds, Color.White);
                IRVector /= 2;
            }

            sb.End();
        }
    }
}
