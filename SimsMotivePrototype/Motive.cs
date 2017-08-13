using System;

namespace SimsMotivePrototype
{
    public class Motive
    {
        //TODO: Rename class...

        public float[] Motives = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public float[] OldMotives = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public Clock Clock { get; set; }

        private Random rand = new Random();

        private void InitMotives()
        {
            for (var i = 0; i < 16; i++)
            {
                Motives[i] = 0;
            }

            Motives[(int)MotiveTypes.Energy] = 70;
            Motives[(int)MotiveTypes.Alertness] = 20;
            Motives[(int)MotiveTypes.Hunger] = -40;

        }

        private void SimMotives(int count)
        {
            var tem = 0f;
            var r = new Rect(100, 100, 140, 140);

            //Clock.AddMinutes(2);

            //Energy
            if (Motives[(int)MotiveTypes.Energy] > 0)
            {
                if (Motives[(int)MotiveTypes.Alertness] > 0)
                    Motives[(int)MotiveTypes.Energy] -= (Motives[(int)MotiveTypes.Alertness] / 100);
                else
                    Motives[(int)MotiveTypes.Energy] -= (Motives[(int)MotiveTypes.Alertness] / 100) * ((100 - Motives[(int)MotiveTypes.Energy] / 50));
            }
            else
            {
                if (Motives[(int)MotiveTypes.Alertness] > 0)
                    Motives[(int)MotiveTypes.Energy] -= (Motives[(int)MotiveTypes.Alertness] / 100) * (100 + Motives[(int)MotiveTypes.Energy] / 50);
                else
                    Motives[(int)MotiveTypes.Energy] -= (Motives[(int)MotiveTypes.Alertness] / 100);
            }

            if (Motives[(int)MotiveTypes.Hunger] > OldMotives[(int)MotiveTypes.Hunger])
            {
                //I had some food
                tem = Motives[(int)MotiveTypes.Hunger] - OldMotives[(int)MotiveTypes.Hunger];
                Motives[(int)MotiveTypes.Energy] += tem / 4;
            }

            //Comfort
            if (Motives[(int)MotiveTypes.Bladder] < 0)
                Motives[(int)MotiveTypes.Comfort] += Motives[(int)MotiveTypes.Bladder] / 10; // max -10

            if (Motives[(int)MotiveTypes.Hygiene] < 0)
                Motives[(int)MotiveTypes.Comfort] += Motives[(int)MotiveTypes.Hygiene] / 20; // max -5

            if (Motives[(int)MotiveTypes.Hunger] < 0)
                Motives[(int)MotiveTypes.Comfort] += Motives[(int)MotiveTypes.Hunger] / 20; // max -5

            // Dec a max 100/cycle in a cubed curve (seek zero)
            Motives[(int)MotiveTypes.Comfort] -= (Motives[(int)MotiveTypes.Comfort] * Motives[(int)MotiveTypes.Comfort] * Motives[(int)MotiveTypes.Comfort]) / 10000;

            //Hunger
            tem = ((Motives[(int)MotiveTypes.Alertness] + 100) / 200) * ((Motives[(int)MotiveTypes.Hunger] + 100) / 100); // ^alert * hunger^0
            Motives[(int)MotiveTypes.Hunger] -= tem;

            if (Motives[(int)MotiveTypes.Stress] < 0) // stress -> hunger
                Motives[(int)MotiveTypes.Hunger] += (Motives[(int)MotiveTypes.Stress] / 100) * ((Motives[(int)MotiveTypes.Hunger] + 100) / 100);

            if (Motives[(int)MotiveTypes.Hunger] < -99)
            {
                AlertCancel("You have starved to death");
                Motives[(int)MotiveTypes.Hunger] = 80;
            }

            //Hygiene
            Motives[(int)MotiveTypes.Hygiene] -= Motives[(int)MotiveTypes.Alertness] > 0 ? 0.3f : 0.1f;

            if (Motives[(int)MotiveTypes.Hygiene] < -97) //Hit limit, bath
            {
                AlertCancel("You smell very bad, mandatory bath");
                Motives[(int)MotiveTypes.Hygiene] = 80;
            }

            //Bladder
            Motives[(int)MotiveTypes.Bladder] -= Motives[(int)MotiveTypes.Alertness] > 0 ? 0.4f : 0.2f; //bladder fills faster while awake

            if (Motives[(int)MotiveTypes.Hunger] > OldMotives[(int)MotiveTypes.Hunger])
            {
                tem = Motives[(int)MotiveTypes.Hunger] - OldMotives[(int)MotiveTypes.Hunger];
                Motives[(int)MotiveTypes.Bladder] -= tem / 4;
            }

            if (Motives[(int)MotiveTypes.Bladder] < -97)
            {
                //Hit limit, gotta go
                if (Motives[(int)MotiveTypes.Alertness] < 0)
                    AlertCancel("You have wet your bed");
                else
                    AlertCancel("You have soiled the carpet");

                Motives[(int)MotiveTypes.Bladder] = 90;
            }

            //Alertness
            if (Motives[(int)MotiveTypes.Alertness] > 0)
                tem = (100 - Motives[(int)MotiveTypes.Alertness]) / 50; //max delta at zero
            else
                tem = (Motives[(int)MotiveTypes.Alertness] + 100) / 50;

            if (Motives[(int)MotiveTypes.Energy] > 0)
            {
                if (Motives[(int)MotiveTypes.Alertness] > 0)
                    Motives[(int)MotiveTypes.Alertness] += (Motives[(int)MotiveTypes.Energy] / 100) * tem;
                else
                    Motives[(int)MotiveTypes.Alertness] += (Motives[(int)MotiveTypes.Energy] / 100);
            }
            else
            {
                if (Motives[(int)MotiveTypes.Alertness] > 0)
                    Motives[(int)MotiveTypes.Alertness] += (Motives[(int)MotiveTypes.Energy] / 100);
                else
                    Motives[(int)MotiveTypes.Alertness] += (Motives[(int)MotiveTypes.Energy] / 100) * tem;
            }

            Motives[(int)MotiveTypes.Alertness] += (Motives[(int)MotiveTypes.Entertained] / 300) * tem;

            if (Motives[(int)MotiveTypes.Bladder] < -50)
                Motives[(int)MotiveTypes.Alertness] -= (Motives[(int)MotiveTypes.Bladder] / 100) * tem;


            //Stress
            Motives[(int)MotiveTypes.Stress] += Motives[(int)MotiveTypes.Comfort] / 10; //max -10
            Motives[(int)MotiveTypes.Stress] += Motives[(int)MotiveTypes.Entertained] / 10; //max -10
            Motives[(int)MotiveTypes.Stress] += Motives[(int)MotiveTypes.Environment] / 15; //max -7
            Motives[(int)MotiveTypes.Stress] += Motives[(int)MotiveTypes.Social] / 20; //max -5

            if (Motives[(int)MotiveTypes.Alertness] < 0) //cut stress while asleep
                Motives[(int)MotiveTypes.Stress] = Motives[(int)MotiveTypes.Stress] / 3;

            //dex a max 100/cycle in a cubed curve (seek zero)
            Motives[(int)MotiveTypes.Stress] -= (Motives[(int)MotiveTypes.Stress] * Motives[(int)MotiveTypes.Stress] * Motives[(int)MotiveTypes.Stress]) / 10000;

            if (Motives[(int)MotiveTypes.Stress] < 0)
            {
                if ((rand.Next(30) - 100) > Motives[(int)MotiveTypes.Stress])
                    if ((rand.Next(30) - 100) > Motives[(int)MotiveTypes.Stress])
                    {
                        AlertCancel("You have lost your temper");
                        ChangeMotive((int)MotiveTypes.Stress, 20);
                    }
            }

            //Environment

            //Social

            //Entertained
            if (Motives[(int)MotiveTypes.Alertness] < 0) // Cut entertained while asleep
                Motives[(int)MotiveTypes.Entertained] = Motives[(int)MotiveTypes.Entertained] / 2;

            //calc physical
            tem = Motives[(int)MotiveTypes.Energy];
            tem += Motives[(int)MotiveTypes.Comfort];
            tem += Motives[(int)MotiveTypes.Hunger];
            tem += Motives[(int)MotiveTypes.Hygiene];
            tem += Motives[(int)MotiveTypes.Bladder];
            tem = tem / 5;

            if (tem > 0)
            {
                //map the linear average into squared curve
                tem = 100 - tem;
                tem = (tem * tem) / 100;
                tem = 100 - tem;
            }
            else
            {
                tem = 100 + tem;
                tem = (tem * tem) / 100;
                tem = tem - 100;
            }

            Motives[(int)MotiveTypes.Physical] = tem;

            //calc mental
            tem = Motives[(int)MotiveTypes.Stress]; // stress counts *2
            tem += Motives[(int)MotiveTypes.Stress];
            tem += Motives[(int)MotiveTypes.Environment];
            tem += Motives[(int)MotiveTypes.Social];
            tem += Motives[(int)MotiveTypes.Entertained];
            tem = tem / 5;

            if (tem > 0)
            {
                //map the linear average into squared curve
                tem = 100 - tem;
                tem = (tem * tem) / 100;
                tem = 100 - tem;
            }
            else
            {
                tem = 100 + tem;
                tem = (tem * tem) / 100;
                tem = tem - 100;
            }

            Motives[(int)MotiveTypes.Mental] = tem;

            //calc and average happiness
            //happy = mental + physical

            Motives[(int)MotiveTypes.HappyNow] = (Motives[(int)MotiveTypes.Physical] + Motives[(int)MotiveTypes.Mental]) / 2;
            Motives[(int)MotiveTypes.HappyDay] = ((Motives[(int)MotiveTypes.HappyDay] * (ConstantsAndConfig.DayTicks - 1)) + Motives[(int)MotiveTypes.HappyNow]) / ConstantsAndConfig.DayTicks;
            Motives[(int)MotiveTypes.HappyWeek] = ((Motives[(int)MotiveTypes.HappyWeek] * (ConstantsAndConfig.WeekTicks - 1)) + Motives[(int)MotiveTypes.HappyNow]) / ConstantsAndConfig.WeekTicks;
            Motives[(int)MotiveTypes.HappyLife] = ((Motives[(int)MotiveTypes.HappyLife] * 9) + Motives[(int)MotiveTypes.HappyWeek]) / 10;

            //Check for over/under flow
            for(var z = 0; z < 16; z++)
            {
                if (Motives[z] > 100)
                    Motives[z] = 100;
                if (Motives[z] < -100)
                    Motives[z] = -100;

                //Save set in oldMotives (for delta tests)
                OldMotives[z] = Motives[z];
            }
        }

        private void AlertCancel (string text)
        {
            //TODO: Do something...
        }

        /// <summary>
        /// Use this to change motives (checks overflow)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void ChangeMotive(int index, float value)
        {
            if (index < 0 || index > Motives.Length - 1) return; //TODO: Log error?

            Motives[index] += value;
            if (Motives[index] > 100)
                Motives[index] = 100;
            if (Motives[index] < -100)
                Motives[index] = -100;
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="type"></param>
        private void SimJob(int type)
        {
            Clock.AddHours(9);

            Motives[(int)MotiveTypes.Energy] = ((Motives[(int)MotiveTypes.Energy] + 100) * 0.3f) - 100;
            Motives[(int)MotiveTypes.Hunger] = -60 + rand.Next(20);
            Motives[(int)MotiveTypes.Hygiene] = -70 + rand.Next(30);
            Motives[(int)MotiveTypes.Bladder] = -50 + rand.Next(50);
            Motives[(int)MotiveTypes.Alertness] = 10 + rand.Next(10);
            Motives[(int)MotiveTypes.Stress] = -50 + rand.Next(50);
        }
    }

    public enum MotiveTypes
    {
        HappyLife = 0,
        HappyWeek = 1,
        HappyDay = 2,
        HappyNow = 3,

        Physical = 4,
        Energy = 5,
        Comfort = 6,
        Hunger = 7,
        Hygiene = 8,
        Bladder = 9,

        Mental = 10,
        Alertness = 11,
        Stress = 12,
        Environment = 13,
        Social = 14,
        Entertained = 15
    }
}
