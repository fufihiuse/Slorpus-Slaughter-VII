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

            // generate rendertargets for mipmaps
            Mips = new RenderTarget2D[passes];
            Point targetSize = size;
            for (int i = 0; i < passes; i++)
            {
                Mips[i] = new RenderTarget2D(GraphicsDevice, targetSize.X, targetSize.Y);
                size /= new Point(2,2);
            }
        }

        public void Apply(RenderTarget2D input, ref RenderTarget2D output)
        {
            GraphicsDevice.SetRenderTarget(output);
        }
    }
}
