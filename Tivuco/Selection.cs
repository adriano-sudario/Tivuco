using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Selection
    {
        Texture2D selectionBoxTexture;
        Rectangle selectionBoxDestinationRectangle;
        Rectangle selectionBoxSourceRectangle;
        Texture2D characterTexture;
        Rectangle characterDestinationRectangle;
        Rectangle characterSourceRectangle;

        public int showTextElapsedTime;
        public bool showText;

        public string playerID;

        string text;
        Vector2 textSize;

        string characterName;
        Vector2 characterNameSize;

        Vector2 position;

        Player.Side side;

        int boxWidth
        {
            get { return 150; }
        }

        int boxHeight
        {
            get { return 260; }
        }

        int characterSelectionWidth
        {
            get { return characterDestinationRectangle.Width; }
        }

        int characterSelectionHeight
        {
            get { return characterDestinationRectangle.Height; }
        }

        float currentCharacterFrame;
        int maxCharacters;

        public Inputs input;
        public Character character;
        public SelectionKind selectionKind;
        //InputSelected inputSelected;

        public enum SelectionKind { InputSelection, CharacterSelection, CharacterSelected }
        public enum Character { Dwarf, Goat, Dove, Skinny }
        //enum InputSelected { None, KeyboardOne, KeyboardTwo, GamePadOne, GamePadTwo, GamePadThree, GamePadFour }

        public Selection(Texture2D selectionBoxTexture, Texture2D characterTexture, int characterWidth, int characterHeight,
                            Rectangle destination, Vector2 position, Player.Side side, string playerID)
        {
            this.position = position;
            this.selectionBoxTexture = selectionBoxTexture;
            this.characterTexture = characterTexture;
            this.selectionBoxDestinationRectangle = destination;
            this.side = side;
            this.playerID = playerID;
            maxCharacters = 4;
            character = Character.Dwarf;
            currentCharacterFrame = (int)character;
            selectionKind = SelectionKind.InputSelection;
            //inputSelected = InputSelected.None;
            text = "Press Start";
            textSize = Game1.font24.MeasureString(text);
            characterName = character.ToString();
            characterNameSize = Game1.font36.MeasureString(characterName);
            showTextElapsedTime = 0;
            showText = true;

            selectionBoxSourceRectangle = new Rectangle(0, 0, boxWidth, boxHeight);
            characterDestinationRectangle = new Rectangle(destination.X, destination.Y + (destination.Y / 2), characterWidth, characterHeight);
            characterSourceRectangle = new Rectangle((int)(currentCharacterFrame * characterSelectionWidth), 0, characterSelectionWidth, characterSelectionHeight);
        }

        public void ResetSelection()
        {
            character = Character.Dwarf;
            currentCharacterFrame = (int)character;
            selectionKind = SelectionKind.InputSelection;
            characterName = character.ToString();
            characterNameSize = Game1.font36.MeasureString(characterName);
            showTextElapsedTime = 0;
            showText = true;
            selectionBoxSourceRectangle = new Rectangle(0, 0, boxWidth, boxHeight);
            characterSourceRectangle = new Rectangle((int)(currentCharacterFrame * characterSelectionWidth), 0, characterSelectionWidth, characterSelectionHeight);
        }

        public void Update(GameTime gameTime)
        {
            switch (selectionKind)
            {
                case SelectionKind.InputSelection:
                    showTextElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (showTextElapsedTime > 249)
                    {
                        if (showText)
                            showText = false;
                        else
                            showText = true;

                        showTextElapsedTime = 0;
                    }
                    break;

                case SelectionKind.CharacterSelection:
                    //input.GetInputs();

                    if (input.SelectX() != 0)
                    {
                        ChangeCharacter(input.SelectX());
                    }

                    if (input.PauseGame())
                    {
                        selectionBoxSourceRectangle.X = boxWidth;
                        selectionKind = SelectionKind.CharacterSelected;
                        Game1.numberOfPlayersSelected++;

                        characterSourceRectangle = new Rectangle((int)((currentCharacterFrame + maxCharacters) * characterSelectionWidth), 0, characterSelectionWidth, characterSelectionHeight);
                    }

                    break;

                case SelectionKind.CharacterSelected:
                    //input.GetInputs();

                    if (input.Back())
                    {
                        selectionBoxSourceRectangle.X = 0;
                        selectionKind = SelectionKind.CharacterSelection;
                        Game1.numberOfPlayersSelected--;

                        characterSourceRectangle = new Rectangle((int)(currentCharacterFrame * characterSelectionWidth), 0, characterSelectionWidth, characterSelectionHeight);
                    }

                    break;
            }
        }

        public void ChangeCharacter(int multiplier)
        {
            currentCharacterFrame += multiplier;

            if (currentCharacterFrame >= maxCharacters)
                currentCharacterFrame = 0;
            else if (currentCharacterFrame < 0)
                currentCharacterFrame = maxCharacters - 1;

            character = (Character)currentCharacterFrame;
            characterSourceRectangle = new Rectangle((int)(currentCharacterFrame * characterSelectionWidth), 0, characterSelectionWidth, characterSelectionHeight);

            characterName = character.ToString();
            characterNameSize = Game1.font36.MeasureString(characterName);
        }

        public void PlayerLoading(ContentManager content)
        {
            switch (character)
            {
                case Character.Dwarf:
                    Game1.players.Add(LoadPlayer(content, 39, 45, 51, 55));
                    break;

                case Character.Goat:
                    Game1.players.Add(LoadPlayer(content, 33, 48, 52, 50));
                    break;

                case Character.Dove:
                    Game1.players.Add(LoadPlayer(content, 42, 51, 54, 54));
                    break;

                case Character.Skinny:
                    Game1.players.Add(LoadPlayer(content, 36, 45, 53, 49));
                    break;
            }
        }

        public Player LoadPlayer(ContentManager content, int width, int height, int deadWidth, int deadHeight)
        {
            if (side == Player.Side.Left)
                position.X -= width;

            Player player;

            Animation playerAnimation = new Animation();
            Texture2D playerTexture = content.Load<Texture2D>("Graphics\\" + characterName.ToLower());
            // Frame dimensions: 39x - 45y
            playerAnimation.Initialize(playerTexture, position, width, height, 0, 0, 100, Color.White, 1f, true);

            Animation deadAnimation = new Animation();
            Texture2D deadTexture = content.Load<Texture2D>("Graphics\\" + characterName.ToLower() + "_flying");
            deadAnimation.Initialize(deadTexture, position, deadWidth, deadHeight, 0, 3, 100, Color.White, 1f, false);

            Texture2D measureBar = content.Load<Texture2D>("Graphics\\measure_bar");
            Texture2D aimTexture = content.Load<Texture2D>("Graphics\\aim");
            Aim aim = new Aim(aimTexture, 30, 9, 50);

            Texture2D appearanceSmokeTexture = content.Load<Texture2D>("Graphics\\appearance_smoking");

            switch (characterName.ToLower())
            {
                case "dwarf":
                    Animation boozeAnimation = new Animation();
                    Texture2D boozeTexture = content.Load<Texture2D>("Graphics\\booze");
                    boozeAnimation.Initialize(boozeTexture, position, 9, 9, 0, 4, 100, Color.White, 1f, true);

                    Animation lighterAnimation = new Animation();
                    Texture2D lighterTexture = content.Load<Texture2D>("Graphics\\lighter");
                    lighterAnimation.Initialize(lighterTexture, position, 4, 7, 0, 4, 100, Color.White, 1f, true);

                    Animation fireSpitAnimation = new Animation();
                    Texture2D fireSpitTexture = content.Load<Texture2D>("Graphics\\lighter_on_fire");
                    fireSpitAnimation.Initialize(fireSpitTexture, position, 46, 32, 0, 4, 100, Color.White, 1f, true);

                    FireSpitting fireSpitting = new FireSpitting(boozeAnimation, lighterAnimation, fireSpitAnimation, 2f, 100, 2f);

                    Texture2D boozeSpinTexture = content.Load<Texture2D>("Graphics\\spinning_booze");

                    //SpinningBooze boozeSpinning = new SpinningBooze(boozeSpinAnimation, 0.3f, 25, 5);

                    player = new Dwarf(playerAnimation, deadAnimation, aim, measureBar, playerID, 100, 100, 0, 4, 5, new Vector2(6, 33),
                                        side, input, fireSpitting, boozeSpinTexture);
                    break;

                case "dove":
                    Animation sandwichAnimation = new Animation();
                    Texture2D sandwichTexture = content.Load<Texture2D>("Graphics\\sandwich");
                    sandwichAnimation.Initialize(sandwichTexture, position, 9, 9, 0, 5, 400, Color.White, 1f, true);

                    Animation stinkAnimation = new Animation();
                    Texture2D stinkTexture = content.Load<Texture2D>("Graphics\\stink");
                    stinkAnimation.Initialize(stinkTexture, position, 45, 45, 0, 6, 100, Color.White, 1f, true);

                    Texture2D pooTexture = content.Load<Texture2D>("Graphics\\poo");

                    Texture2D forkTexture = content.Load<Texture2D>("Graphics\\fork");
                    Animation forkAnimation = new Animation();
                    forkAnimation.Initialize(forkTexture, new Vector2(0, 0), 18, 18, 0, 7, 50, Color.White, 1f, true);

                    Projectile fork = new Projectile(forkAnimation, 0, 30, 10);

                    player = new Dove(playerAnimation, deadAnimation, aim, measureBar, playerID, 100, 100, 0.5f, 5, 5, new Vector2(3, 39),
                                        side, input, sandwichAnimation, pooTexture, 4, stinkAnimation, fork);
                    break;

                case "skinny":
                    Animation spaceshipAnimation = new Animation();
                    Texture2D spaceshipTexture = content.Load<Texture2D>("Graphics\\spaceship");
                    spaceshipAnimation.Initialize(spaceshipTexture, position, 57, 30, 0, 4, 50, Color.White, 1f, true);

                    Texture2D spaceshoot = content.Load<Texture2D>("Graphics\\spaceshoot");
                    Texture2D crazyRay = content.Load<Texture2D>("Graphics\\crazy_ray");

                    Animation disappearanceAnimation = new Animation();
                    Texture2D disappearanceTexture = content.Load<Texture2D>("Graphics\\skinny_teleport");
                    disappearanceAnimation.Initialize(disappearanceTexture, position, width, height, 0, 6, 50, Color.White, 1f, true);

                    Animation appearanceSmokeAnimation = new Animation();
                    appearanceSmokeAnimation.Initialize(appearanceSmokeTexture, position, 57, 57, 0, 8, 40, Color.White, 1f, true);

                    Teleport tp = new Teleport(disappearanceAnimation, appearanceSmokeAnimation, 100f);

                    player = new Skinny(playerAnimation, deadAnimation, aim, measureBar, playerID, 100, 100, 0.2f, 5, 5, new Vector2(6, 30),
                                        side, input, spaceshipAnimation, spaceshoot, crazyRay, tp, 0.7f);
                    break;

                case "goat":
                    Animation disillusionRaysAnimation = new Animation();
                    Texture2D disillusionRaysTexture = content.Load<Texture2D>("Graphics\\appearance_ray");
                    disillusionRaysAnimation.Initialize(disillusionRaysTexture, position, 33, 48, 0, 3, 50, Color.White, 1f, true);

                    Texture2D heart = content.Load<Texture2D>("Graphics\\heart");
                    Texture2D disconnected = content.Load<Texture2D>("Graphics\\disconnected");

                    Love luv = new Love(heart, disillusionRaysAnimation);
                    RoutesConfiguration routesConfig = new RoutesConfiguration(appearanceSmokeTexture);

                    player = new Goat(playerAnimation, deadAnimation, aim, measureBar, playerID, 100, 100, 0.15f, 5, 5, new Vector2(6, 30),
                                        side, input, disconnected, luv, routesConfig);
                    break;

                default:
                    player = null;
                    break;
            }

            return player;
        }

        public void AddRoundsStatisticsToPlayer(ContentManager content)
        {
            Texture2D roundBox = content.Load<Texture2D>("Graphics\\round_box");
            Texture2D roundCharacters = content.Load<Texture2D>("Graphics\\round_characters");
            Texture2D roundCount = content.Load<Texture2D>("Graphics\\round_counts");

            Vector2 roundBoxPosition = new Vector2((Game1.screenWidth / 2) - (roundBox.Width / 2), 
                                                    (Game1.screenHeight / 2) - (roundBox.Height / 2));

            Game1.result.AddRound(roundBox, roundCount, roundCharacters, 35, 65, roundBoxPosition, playerID, character);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (selectionKind)
            {
                case SelectionKind.InputSelection:
                    spriteBatch.Draw(selectionBoxTexture, selectionBoxDestinationRectangle, selectionBoxSourceRectangle, Color.White);
                    if (showText)
                        spriteBatch.DrawString(Game1.font24, text, new Vector2(selectionBoxDestinationRectangle.X + (boxWidth / 2), selectionBoxDestinationRectangle.Y + (boxHeight / 2)), Color.White, 0, new Vector2(textSize.X / 2, textSize.Y / 2), 1f, SpriteEffects.None, 0);
                    break;

                case SelectionKind.CharacterSelection:
                case SelectionKind.CharacterSelected:
                    spriteBatch.Draw(selectionBoxTexture, selectionBoxDestinationRectangle, selectionBoxSourceRectangle, Color.White);
                    spriteBatch.Draw(characterTexture, characterDestinationRectangle, characterSourceRectangle, Color.White);

                    spriteBatch.DrawString(Game1.font36, characterName, new Vector2(selectionBoxDestinationRectangle.X + (boxWidth / 2), selectionBoxDestinationRectangle.Y + selectionBoxDestinationRectangle.Height), Color.White, 0, new Vector2(characterNameSize.X / 2, 0), 1f, SpriteEffects.None, 0);
                    break;
            }
        }
    }
}
