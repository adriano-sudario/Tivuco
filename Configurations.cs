using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Configurations
    {
        public bool fullScreen;
        public bool muteSounds;
        public float soundsVolume;

        public Configurations(bool fullScreen, bool muteSounds, float soundsVolume)
        {
            this.fullScreen = fullScreen;
            this.muteSounds = muteSounds;
            this.soundsVolume = soundsVolume;
        }
    }
}
