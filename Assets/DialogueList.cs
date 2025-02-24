using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class DialogueList:Queue<string>
    {
        private int maxCount = 10;

        public void Push(string dialogue)
        {
            if (this.Count > maxCount)
            {
                this.Dequeue();
            }
            this.Enqueue(dialogue);
        }
    }
}
