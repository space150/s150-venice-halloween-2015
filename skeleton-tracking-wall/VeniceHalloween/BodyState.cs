using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeniceHalloween
{
    public class BodyState
    {
        public bool HasLeftHandWeapon;
        public int LeftHandWeaponIndex;

        public bool HasRightHandWeapon;
        public int RightHandWeaponIndex;

        private int weaponCount;

        public BodyState(int count)
        {
            this.weaponCount = count;

            this.HasLeftHandWeapon = false;
            this.LeftHandWeaponIndex = -1;

            this.HasRightHandWeapon = false;
            this.RightHandWeaponIndex = -1;
        }

        public void Update(TrackingConfidence leftConfidence, HandState leftHand, TrackingConfidence rightConfidence, HandState rightHand)
        {
            if ( leftConfidence == TrackingConfidence.High && leftHand == HandState.Closed )
            {
                if (!this.HasLeftHandWeapon)
                {
                    this.LeftHandWeaponIndex = this.selectRandomWeapon();
                    this.HasLeftHandWeapon = true;
                }
            }
            else
            {
                this.HasLeftHandWeapon = false;
                this.LeftHandWeaponIndex = -1;
            }

            if (rightConfidence == TrackingConfidence.High && rightHand == HandState.Closed)
            {
                if (!this.HasRightHandWeapon)
                {
                    this.RightHandWeaponIndex = this.selectRandomWeapon();
                    this.HasRightHandWeapon = true;
                }
            }
            else
            {
                this.HasRightHandWeapon = false;
                this.RightHandWeaponIndex = -1;
            }
        }

        private int selectRandomWeapon()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(0, this.weaponCount);
        }

    }
}
