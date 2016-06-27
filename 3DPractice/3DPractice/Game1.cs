using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3DPractice
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        enum StretchAxis {X, Y, Z, None}
        enum StretchDirection {Positive, Negative, None}
        StretchAxis currentStretchAxis = StretchAxis.None;
        StretchDirection currentStretchDirection = StretchDirection.None;
        GraphicsDeviceManager graphics;

        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix; // camera's lens
        Matrix viewMatrix;       // camera's position
        Matrix worldMatrix;      // object's position

        BasicEffect basicEffect;

        // Geometric Info
        VertexBuffer vertexBuffer;  // use to draw to graphics card
        VertexPositionColor[] allCubes;
        VertexPositionColor[] colorCubeVertices;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            // setup camera
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(0f, 0f, -100f);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45f),
                GraphicsDevice.DisplayMode.AspectRatio, 1f, 10000f);

            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            // BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;

            // create triangle
            colorCubeVertices = GetCubeVertices(0, 0, 0, 40, 40, 40);
            VertexPositionColor[] xAxisVertices = GetCubeVertices(0, 0, 0, 20000, 1, 1, Color.DarkRed);
            VertexPositionColor[] yAxisVertices = GetCubeVertices(0, 0, 0, 1, 20000, 1, Color.DarkBlue);
            VertexPositionColor[] zAxisVertices = GetCubeVertices(0, 0, 0, 1, 1, 20000, Color.DarkGreen);

            VertexPositionColor[] colorCube = ConstructCube(colorCubeVertices, new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple});
            VertexPositionColor[] xAxis = ConstructCube(xAxisVertices);
            VertexPositionColor[] yAxis = ConstructCube(yAxisVertices);
            VertexPositionColor[] zAxis = ConstructCube(zAxisVertices);

            allCubes = new VertexPositionColor[colorCube.Length + xAxis.Length + yAxis.Length + zAxis.Length];

            colorCube.CopyTo(allCubes, 0);
            xAxis.CopyTo(allCubes, colorCube.Length);
            yAxis.CopyTo(allCubes, colorCube.Length+ xAxis.Length);
            zAxis.CopyTo(allCubes, colorCube.Length + xAxis.Length + yAxis.Length);

            Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(45f));
            camPosition = Vector3.Transform(camPosition, rotationMatrix);
            camTarget = Vector3.Transform(camTarget, rotationMatrix);

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), allCubes.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(allCubes);
        }

        private VertexPositionColor[] GetCubeVertices(float x, float y, float z, float lengthX, float lengthY, float lengthZ)
        {
            VertexPositionColor[] cube = new VertexPositionColor[8];

            cube[0] = new VertexPositionColor(new Vector3(x + lengthX / 2, y + lengthY / 2, z + lengthZ / 2), Color.Red);
            cube[1] = new VertexPositionColor(new Vector3(x + lengthX / 2, y + lengthY / 2, z - lengthZ / 2), Color.Orange);
            cube[2] = new VertexPositionColor(new Vector3(x + lengthX / 2, y - lengthY / 2, z + lengthZ / 2), Color.Yellow);
            cube[3] = new VertexPositionColor(new Vector3(x + lengthX / 2, y - lengthY / 2, z - lengthZ / 2), Color.Green);
            cube[4] = new VertexPositionColor(new Vector3(x - lengthX / 2, y + lengthY / 2, z + lengthZ / 2), Color.Teal);
            cube[5] = new VertexPositionColor(new Vector3(x - lengthX / 2, y + lengthY / 2, z - lengthZ / 2), Color.Blue);
            cube[6] = new VertexPositionColor(new Vector3(x - lengthX / 2, y - lengthY / 2, z + lengthZ / 2), Color.Purple);
            cube[7] = new VertexPositionColor(new Vector3(x - lengthX / 2, y - lengthY / 2, z - lengthZ / 2), Color.DarkRed);

            return cube;
        }

        private VertexPositionColor[] GetCubeVertices(float x, float y, float z, float lengthX, float lengthY, float lengthZ, Color color)
        {
            VertexPositionColor[] cube = new VertexPositionColor[8];

            cube[0] = new VertexPositionColor(new Vector3(x + lengthX / 2, y + lengthY / 2, z + lengthZ / 2), color);
            cube[1] = new VertexPositionColor(new Vector3(x + lengthX / 2, y + lengthY / 2, z - lengthZ / 2), color);
            cube[2] = new VertexPositionColor(new Vector3(x + lengthX / 2, y - lengthY / 2, z + lengthZ / 2), color);
            cube[3] = new VertexPositionColor(new Vector3(x + lengthX / 2, y - lengthY / 2, z - lengthZ / 2), color);
            cube[4] = new VertexPositionColor(new Vector3(x - lengthX / 2, y + lengthY / 2, z + lengthZ / 2), color);
            cube[5] = new VertexPositionColor(new Vector3(x - lengthX / 2, y + lengthY / 2, z - lengthZ / 2), color);
            cube[6] = new VertexPositionColor(new Vector3(x - lengthX / 2, y - lengthY / 2, z + lengthZ / 2), color);
            cube[7] = new VertexPositionColor(new Vector3(x - lengthX / 2, y - lengthY / 2, z - lengthZ / 2), color);

            return cube;
        }

        private VertexPositionColor[] ConstructCube(VertexPositionColor[] vertices)
        {
            VertexPositionColor[] cubeTriangulated = new VertexPositionColor[36];

            cubeTriangulated[0] = vertices[0];
            cubeTriangulated[1] = vertices[1];
            cubeTriangulated[2] = vertices[3];

            cubeTriangulated[3] = vertices[0];
            cubeTriangulated[4] = vertices[2];
            cubeTriangulated[5] = vertices[3];

            cubeTriangulated[6] = vertices[0];
            cubeTriangulated[7] = vertices[1];
            cubeTriangulated[8] = vertices[5];

            cubeTriangulated[9] = vertices[0];
            cubeTriangulated[10] = vertices[4];
            cubeTriangulated[11] = vertices[5];

            cubeTriangulated[12] = vertices[0];
            cubeTriangulated[13] = vertices[2];
            cubeTriangulated[14] = vertices[6];

            cubeTriangulated[15] = vertices[0];
            cubeTriangulated[16] = vertices[4];
            cubeTriangulated[17] = vertices[6];

            cubeTriangulated[18] = vertices[7];
            cubeTriangulated[19] = vertices[6];
            cubeTriangulated[20] = vertices[2];

            cubeTriangulated[21] = vertices[7];
            cubeTriangulated[22] = vertices[3];
            cubeTriangulated[23] = vertices[2];

            cubeTriangulated[24] = vertices[7];
            cubeTriangulated[25] = vertices[6];
            cubeTriangulated[26] = vertices[4];

            cubeTriangulated[27] = vertices[7];
            cubeTriangulated[28] = vertices[5];
            cubeTriangulated[29] = vertices[4];

            cubeTriangulated[30] = vertices[7];
            cubeTriangulated[31] = vertices[5];
            cubeTriangulated[32] = vertices[1];

            cubeTriangulated[33] = vertices[7];
            cubeTriangulated[34] = vertices[3];
            cubeTriangulated[35] = vertices[1];

            return cubeTriangulated;
        }

        private VertexPositionColor[] ConstructCube(VertexPositionColor[] vertices, Color[] colors)
        {
            VertexPositionColor[] cubeTriangulated = new VertexPositionColor[36];

            cubeTriangulated[0] = vertices[0];
            cubeTriangulated[1] = vertices[1];
            cubeTriangulated[2] = vertices[3];

            cubeTriangulated[3] = vertices[0];
            cubeTriangulated[4] = vertices[2];
            cubeTriangulated[5] = vertices[3];

            cubeTriangulated[6] = vertices[0];
            cubeTriangulated[7] = vertices[1];
            cubeTriangulated[8] = vertices[5];

            cubeTriangulated[9] = vertices[0];
            cubeTriangulated[10] = vertices[4];
            cubeTriangulated[11] = vertices[5];

            cubeTriangulated[12] = vertices[0];
            cubeTriangulated[13] = vertices[2];
            cubeTriangulated[14] = vertices[6];

            cubeTriangulated[15] = vertices[0];
            cubeTriangulated[16] = vertices[4];
            cubeTriangulated[17] = vertices[6];

            cubeTriangulated[18] = vertices[7];
            cubeTriangulated[19] = vertices[6];
            cubeTriangulated[20] = vertices[2];

            cubeTriangulated[21] = vertices[7];
            cubeTriangulated[22] = vertices[3];
            cubeTriangulated[23] = vertices[2];

            cubeTriangulated[24] = vertices[7];
            cubeTriangulated[25] = vertices[6];
            cubeTriangulated[26] = vertices[4];

            cubeTriangulated[27] = vertices[7];
            cubeTriangulated[28] = vertices[5];
            cubeTriangulated[29] = vertices[4];

            cubeTriangulated[30] = vertices[7];
            cubeTriangulated[31] = vertices[5];
            cubeTriangulated[32] = vertices[1];

            cubeTriangulated[33] = vertices[7];
            cubeTriangulated[34] = vertices[3];
            cubeTriangulated[35] = vertices[1];

            for(int i = 0; i < 36; i++)
            {
                cubeTriangulated[i].Color = colors[i / 6];
            }

            return cubeTriangulated;
        }

        protected override void LoadContent()
        {

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // Left, Right, Up, Down
            if(Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                camPosition.X -= 1f;
                camTarget.X -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                camPosition.X += 1f;
                camTarget.X += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                camPosition.Y -= 1f;
                camTarget.Y -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                camPosition.Y += 1f;
                camTarget.Y += 1f;
            }

            // Plus, Minus
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                camPosition.Z += 1f;
                camTarget.Z += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                camPosition.Z -= 1f;
                camTarget.Z -= 1f;
            }

            // Space
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Matrix rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
                camPosition = Vector3.Transform(camPosition, rotationMatrix);
                camTarget = Vector3.Transform(camTarget, rotationMatrix);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                currentStretchAxis = StretchAxis.X;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                currentStretchAxis = StretchAxis.Y;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                currentStretchAxis = StretchAxis.Z;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                currentStretchDirection = StretchDirection.Positive;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                currentStretchDirection = StretchDirection.Negative;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                int sign;
                switch(currentStretchDirection)
                {
                    case StretchDirection.Positive:
                        sign = 1;
                        break;
                    case StretchDirection.Negative:
                        sign = -1;
                        break;
                    case StretchDirection.None:
                    default:
                        sign = 0;
                        break;
                }
                switch(currentStretchAxis)
                {
                    case StretchAxis.X:
                        colorCubeVertices[0].Position.X += sign * 1;
                        break;
                    case StretchAxis.Y:
                        colorCubeVertices[0].Position.Y += sign * 1;
                        break;
                    case StretchAxis.Z:
                        colorCubeVertices[0].Position.Z += sign * 1;
                        break;
                    case StretchAxis.None:
                    default:
                        break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
            {
                int sign;
                switch (currentStretchDirection)
                {
                    case StretchDirection.Positive:
                        sign = 1;
                        break;
                    case StretchDirection.Negative:
                        sign = -1;
                        break;
                    case StretchDirection.None:
                    default:
                        sign = 0;
                        break;
                }
                switch (currentStretchAxis)
                {
                    case StretchAxis.X:
                        colorCubeVertices[1].Position.X += sign * 1;
                        break;
                    case StretchAxis.Y:
                        colorCubeVertices[1].Position.Y += sign * 1;
                        break;
                    case StretchAxis.Z:
                        colorCubeVertices[1].Position.Z += sign * 1;
                        break;
                    case StretchAxis.None:
                    default:
                        break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad3))
            {
                int sign;
                switch (currentStretchDirection)
                {
                    case StretchDirection.Positive:
                        sign = 1;
                        break;
                    case StretchDirection.Negative:
                        sign = -1;
                        break;
                    case StretchDirection.None:
                    default:
                        sign = 0;
                        break;
                }
                switch (currentStretchAxis)
                {
                    case StretchAxis.X:
                        colorCubeVertices[2].Position.X += sign * 1;
                        break;
                    case StretchAxis.Y:
                        colorCubeVertices[2].Position.Y += sign * 1;
                        break;
                    case StretchAxis.Z:
                        colorCubeVertices[2].Position.Z += sign * 1;
                        break;
                    case StretchAxis.None:
                    default:
                        break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
            {
                int sign;
                switch (currentStretchDirection)
                {
                    case StretchDirection.Positive:
                        sign = 1;
                        break;
                    case StretchDirection.Negative:
                        sign = -1;
                        break;
                    case StretchDirection.None:
                    default:
                        sign = 0;
                        break;
                }
                switch (currentStretchAxis)
                {
                    case StretchAxis.X:
                        colorCubeVertices[3].Position.X += sign * 1;
                        break;
                    case StretchAxis.Y:
                        colorCubeVertices[3].Position.Y += sign * 1;
                        break;
                    case StretchAxis.Z:
                        colorCubeVertices[3].Position.Z += sign * 1;
                        break;
                    case StretchAxis.None:
                    default:
                        break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5))
            {
                int sign;
                switch (currentStretchDirection)
                {
                    case StretchDirection.Positive:
                        sign = 1;
                        break;
                    case StretchDirection.Negative:
                        sign = -1;
                        break;
                    case StretchDirection.None:
                    default:
                        sign = 0;
                        break;
                }
                switch (currentStretchAxis)
                {
                    case StretchAxis.X:
                        colorCubeVertices[4].Position.X += sign * 1;
                        break;
                    case StretchAxis.Y:
                        colorCubeVertices[4].Position.Y += sign * 1;
                        break;
                    case StretchAxis.Z:
                        colorCubeVertices[4].Position.Z += sign * 1;
                        break;
                    case StretchAxis.None:
                    default:
                        break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
            {
                int sign;
                switch (currentStretchDirection)
                {
                    case StretchDirection.Positive:
                        sign = 1;
                        break;
                    case StretchDirection.Negative:
                        sign = -1;
                        break;
                    case StretchDirection.None:
                    default:
                        sign = 0;
                        break;
                }
                switch (currentStretchAxis)
                {
                    case StretchAxis.X:
                        colorCubeVertices[5].Position.X += sign * 1;
                        break;
                    case StretchAxis.Y:
                        colorCubeVertices[5].Position.Y += sign * 1;
                        break;
                    case StretchAxis.Z:
                        colorCubeVertices[5].Position.Z += sign * 1;
                        break;
                    case StretchAxis.None:
                    default:
                        break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad7))
            {
                int sign;
                switch (currentStretchDirection)
                {
                    case StretchDirection.Positive:
                        sign = 1;
                        break;
                    case StretchDirection.Negative:
                        sign = -1;
                        break;
                    case StretchDirection.None:
                    default:
                        sign = 0;
                        break;
                }
                switch (currentStretchAxis)
                {
                    case StretchAxis.X:
                        colorCubeVertices[6].Position.X += sign * 1;
                        break;
                    case StretchAxis.Y:
                        colorCubeVertices[6].Position.Y += sign * 1;
                        break;
                    case StretchAxis.Z:
                        colorCubeVertices[6].Position.Z += sign * 1;
                        break;
                    case StretchAxis.None:
                    default:
                        break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
            {
                int sign;
                switch (currentStretchDirection)
                {
                    case StretchDirection.Positive:
                        sign = 1;
                        break;
                    case StretchDirection.Negative:
                        sign = -1;
                        break;
                    case StretchDirection.None:
                    default:
                        sign = 0;
                        break;
                }
                switch (currentStretchAxis)
                {
                    case StretchAxis.X:
                        colorCubeVertices[7].Position.X += sign * 1;
                        break;
                    case StretchAxis.Y:
                        colorCubeVertices[7].Position.Y += sign * 1;
                        break;
                    case StretchAxis.Z:
                        colorCubeVertices[7].Position.Z += sign * 1;
                        break;
                    case StretchAxis.None:
                    default:
                        break;
                }
            }

            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

            VertexPositionColor[] colorCube = ConstructCube(colorCubeVertices, new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple });

            colorCube.CopyTo(allCubes, 0);
            vertexBuffer.SetData<VertexPositionColor>(allCubes);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            basicEffect.Projection = projectionMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;

            GraphicsDevice.Clear(Color.White);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            // Turn off back face culling
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach(EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 48);
            }
            
            base.Draw(gameTime);
        }
    }
}
