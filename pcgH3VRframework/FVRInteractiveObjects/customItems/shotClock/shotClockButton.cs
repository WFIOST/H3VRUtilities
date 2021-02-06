using FistVR;

namespace H3VRUtils.customItems.shotClock
{
    internal class shotClockButton : FVRPhysicalObject
    {
        public enum buttonType
        {
            start,
            stop,
            register,
            delay
        }

        public shotClock shotclock;

        public bool pressed;
        public buttonType button;

        public void Update()
        {
            if (pressed)
            {
                pressed = false;
                SimpleInteraction(null);
            }
        }

        public override void SimpleInteraction(FVRViveHand hand)
        {
            switch (button)
            {
                case buttonType.start:
                    shotclock.startClockProcess();
                    break;
                case buttonType.stop:
                    shotclock.StopClock();
                    break;
                case buttonType.register:

                    break;
                case buttonType.delay:

                    break;
            }
        }
    }
}