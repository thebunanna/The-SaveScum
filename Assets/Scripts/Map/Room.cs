using System.Collections;
using System.Collections.Generic;
public struct Room {
        string T;
        int X,Y;
        public List<Entity> Ent;
        bool[] doors;
        public Room (string type, int x, int y, List<Entity> Entities, bool[] isOpen) {
            T = type;
            X = x;
            Y = y;
            Ent = Entities;
            doors = isOpen;
        }
        public Room (Room before, List<Entity> entities) {
            T = before.T;
            X = before.X;
            Y = before.Y;
            Ent = entities;
            doors = before.doors;
        }
        public int getX () {return X;}
        public int getY () {return Y;}
        public bool N() {return doors[0];}
        public bool S() {return doors[1];}
        public bool E() {return doors[2];}
        public bool W() {return doors[3];}

        public bool[] Doors() {return doors;}

        public override string ToString() => $"{X}, {Y}";
    }