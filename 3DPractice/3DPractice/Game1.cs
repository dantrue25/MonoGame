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
        GraphicsDeviceManager graphics;

        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix; // camera's lens
        Matrix viewMatrix;       // camera's position
        Matrix worldMatrix;      // object's position

        BasicEffect basicEffect;

        // Geometric Info
        VertexPositionColor[] cube; // position and color of trangle's vertices
        VertexBuffer vertexBuffer;              // use to draw to graphics card

        // Orbit
        bool orbit;

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
                GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);

            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            // BasicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;

            // create triangle
            cube = new VertexPositionColor[8];
            cube[0] = new VertexPositionColor(new Vector3(20, 20, 20), Color.Red);
            cube[1] = new VertexPositionColor(new Vector3(20, 20, -20), Color.Orange);
            cube[2] = new VertexPositionColor(new Vector3(20, -20, 20), Color.Yellow);
            cube[3] = new VertexPositionColor(new Vector3(20, -20, -20), Color.Green);
            cube[4] = new VertexPositionColor(new Vector3(-20, 20, 20), Color.Teal);
            cube[5] = new VertexPositionColor(new Vector3(-20, 20, -20), Color.Blue);
            cube[6] = new VertexPositionColor(new Vector3(-20, -20, 20), Color.Purple);
            cube[7] = new VertexPositionColor(new Vector3(-20, -20, -20), Color.DarkRed);

            VertexPositionColor[] cubeTriangulated = new VertexPositionColor[36];

            cubeTriangulated[0] = cube[0];
            cubeTriangulated[1] = cube[1];
            cubeTriangulated[2] = cube[3];

            cubeTriangulated[3] = cube[0];
            cubeTriangulated[4] = cube[2];
            cubeTriangulated[5] = cube[3];

            cubeTriangulated[6] = cube[0];
            cubeTriangulated[7] = cube[1];
            cubeTriangulated[8] = cube[5];

            cubeTriangulated[9] = cube[0];
            cubeTriangulated[10] = cube[4];
            cubeTriangulated[11] = cube[5];

            cubeTriangulated[12] = cube[0];
            cubeTriangulated[13] = cube[2];
            cubeTriangulated[14] = cube[6];

            cubeTriangulated[15] = cube[0];
            cubeTriangulated[16] = cube[4];
            cubeTriangulated[17] = cube[6];

            cubeTriangulated[18] = cube[7];
            cubeTriangulated[19] = cube[6];
            cubeTriangulated[20] = cube[2];

            cubeTriangulated[21] = cube[7];
            cubeTriangulated[22] = cube[3];
            cubeTriangulated[23] = cube[2];

            cubeTriangulated[24] = cube[7];
            cubeTriangulated[25] = cube[6];
            cubeTriangulated[26] = cube[4];

            cubeTriangulated[27] = cube[7];
            cubeTriangulated[28] = cube[5];
            cubeTriangulated[29] = cube[4];

            cubeTriangulated[30] = cube[7];
            cubeTriangulated[31] = cube[5];
            cubeTriangulated[32] = cube[1];

            cubeTriangulated[33] = cube[7];
            cubeTriangulated[34] = cube[3];
            cubeTriangulated[35] = cube[1];

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 36, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(cubeTriangulated);
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
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                camPosition.Z -= 1f;
            }

            // Space
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                orbit = !orbit;
            }

            if(orbit)
            {
                Matrix rotationMatrix = Matrix.CreateRotationY(
                    MathHelper.ToRadians(1f));
                camPosition = Vector3.Transform(camPosition, rotationMatrix);
            }

            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);

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
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 3, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 6, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 9, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 12, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 15, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 18, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 21, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 24, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 27, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 30, 3);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 33, 3);
            }
            
            base.Draw(gameTime);
        }
    }
}
