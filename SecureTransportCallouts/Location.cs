using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace SecureTransportCallouts
{
    class Location
    {

        private Vector3 vector;
        private float heading;

        public Location(Vector3 vector, float heading)
        {
            this.vector = vector;
            this.heading = heading;
        }

        public Vector3 getVector()
        {
            return vector;
        }

        public float getHeading()
        {
            return heading;
        }

    }
}
