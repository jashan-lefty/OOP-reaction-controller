using System;

namespace EnhancedReactionMachine
{
    public class EnhancedReactionController : IController
    {
        public IGui gui;
        public IRandom randomNumberGenerator;
        public State currentState;
        public int tickCount;
        public int gamesPlayed;
        public double totalTime; // Total reaction time across all games

        public void CoinInserted()
        {
            currentState.CoinInserted();
        }

        public void Connect(IGui gui, IRandom rng)
        {
            this.gui = gui;
            this.randomNumberGenerator = rng;
            Init();
        }

        public void GoStopPressed()
        {
            currentState.GoStopPressed();
        }

        public void Init()
        {
            currentState = new InsertCoinState(this);
            gamesPlayed = 1;
            totalTime = 0;
        }

        public void Tick()
        {
            currentState.Tick();
        }
    }

    public class InsertCoinState : State
    {
        public InsertCoinState(EnhancedReactionController controller) : base(controller)
        {
            controller.gui.SetDisplay("Insert Coin");
        }

        public override void CoinInserted()
        {
            controller.currentState = new StartState(controller);
        }

        public override void GoStopPressed()
        {
            controller.currentState = this;
        }

        public override void Tick()
        {
            // Nothing to do here
        }
    }

    public class StartState : State
    {
        public StartState(EnhancedReactionController controller) : base(controller)
        {
            controller.gui.SetDisplay("Press GO!");
        }

        public override void CoinInserted()
        {
            // Nothing to do here
        }

        public override void GoStopPressed()
        {
            controller.currentState = new RandomDelayState(controller);
        }

        public override void Tick()
        {
            controller.tickCount++;
            if (controller.tickCount == 1000)
            {
                controller.currentState = new GameOverState(controller);
            }
        }
    }

    public class RandomDelayState : State
    {
        private int wait;

        public RandomDelayState(EnhancedReactionController controller) : base(controller)
        {
            controller.tickCount = 0;
            controller.gui.SetDisplay("Wait...");
            wait = controller.randomNumberGenerator.GetRandom(100, 250);
        }

        public override void CoinInserted()
        {
            // Nothing to do here
        }

        public override void GoStopPressed()
        {
            controller.currentState = new InsertCoinState(controller);
        }

        public override void Tick()
        {
            controller.tickCount++;
            if (controller.tickCount == wait)
            {
                //controller.tickCount = 0; // Reset tick count
                controller.currentState = new RunningState(controller);
            }
        }

    }

    public class RunningState : State
    {
        public RunningState(EnhancedReactionController controller) : base(controller)
        {
            controller.tickCount = 0;
            controller.gui.SetDisplay("0.00");
        }

        public override void CoinInserted()
        {
            // Nothing to do here
        }

        public override void GoStopPressed()
        {
            controller.totalTime += (controller.tickCount );
            if (controller.gamesPlayed < 2)
            {
                controller.gamesPlayed++;
                controller.currentState = new DisplayState(controller);
            }
            else
            {
                controller.currentState = new GameOverState(controller);
            }
        }

        public override void Tick()
        {
            controller.tickCount++;
            controller.gui.SetDisplay((controller.tickCount / 100.0).ToString("0.00"));
            
            if (controller.tickCount == 200)
            {
                controller.totalTime += (controller.tickCount );
                controller.tickCount = 0; // Reset tick count
                controller.currentState = new GameOverState(controller);
            }
        }

    }

    public class GameOverState : State
    {
        public GameOverState(EnhancedReactionController controller) : base(controller)
        {
            controller.tickCount = 0;
            double averageTime =  ((double)controller.totalTime / controller.gamesPlayed * 0.01);
            controller.gui.SetDisplay($"Average = {averageTime.ToString("0.00")}");

           
        }
        public override void CoinInserted()
        {
            // Nothing to do here
        }

        public override void GoStopPressed()
        {
            controller.currentState = new InsertCoinState(controller);
        }

        public override void Tick()
        {
            controller.tickCount++;

            if (controller.tickCount == 500)
            {
                controller.gamesPlayed = 0;
                controller.totalTime = 0;
                controller.currentState = new InsertCoinState(controller);
            }
        }
    }

    public class DisplayState : State
    {
        

        public DisplayState(EnhancedReactionController controller) : base(controller)
        {
            controller.tickCount = 0; // Reset tick count.
        }

        public override void CoinInserted()
        {
            // Nothing to do here
        }

        public override void GoStopPressed()
        {
            controller.currentState = new RandomDelayState(controller);
        }

        public override void Tick()
        {
            controller.tickCount++;

            if (controller.tickCount == 300)
            {
             controller.currentState = new RandomDelayState(controller);
            }
        }
    }


    public abstract class State
    {
        protected EnhancedReactionController controller;

        public State(EnhancedReactionController controller)
        {
            this.controller = controller;
        }

        public abstract void CoinInserted();

        public abstract void GoStopPressed();

        public abstract void Tick();
    }
}
