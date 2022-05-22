namespace ProducerConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            Random rand = new Random();
            int storageSize = rand.Next(2, 5);
            int itemNumbers = rand.Next(3, 8);
            int consumers = rand.Next(2, 7);
            int producers = consumers;
            Console.WriteLine("Storage size: {0}\nNumber of items: {1}\nConsumers: {2}\nProducers: {3}\n", storageSize, itemNumbers, consumers, producers);
            program.Starter(storageSize, itemNumbers, consumers, producers);
        }

        private void Starter(int storageSize, int itemNumbers, int consumers, int producers)
        {
            Access = new Semaphore(1, 1);
            Full = new Semaphore(storageSize, storageSize);//3, 3
            Empty = new Semaphore(0, storageSize);//0, 3
            int all = consumers + producers;
            for (int i = 0; i < all; i++)
            {
                if (consumers > 0)
                {
                    Thread threadConsumer = new Thread(Consumer);
                    threadConsumer.Name = "Consumer " + (i + 1);
                    Args args = new Args(itemNumbers, threadConsumer);
                    threadConsumer.Start(args);
                    consumers--;
                }
                if (producers > 0)
                {
                    Thread threadProducer = new Thread(Producer);
                    threadProducer.Name = "Producer " + (i + 1);
                    Args args = new Args(itemNumbers, threadProducer);
                    threadProducer.Start(args);
                    producers--;
                }
            }
        }
        class Args
        {
            public int ItemNumbers { get; set; }
            public Thread Thread { get; set; }

            public Args(int itemNumbers, Thread thread)
            {
                ItemNumbers = itemNumbers;
                Thread = thread;
            }
        }
        private Semaphore Access;
        private Semaphore Full;
        private Semaphore Empty;
        private int Items = 1;

        private readonly List<string> storage = new List<string>();


        private void Producer(Object input)
        {
            int maxItem = 0;
            Args args = (Args)input;

            maxItem = args.ItemNumbers;

            for (int i = 0; i < maxItem; i++)
            {
                Full.WaitOne();
                Access.WaitOne();
                storage.Add("item " + Items);
                Console.WriteLine(" + Added item " + Items + " by " + args.Thread.Name);
                Items++;
                Access.Release();
                Empty.Release();

            }
        }

        private void Consumer(Object input)
        {
            int maxItem = 0;
            Args args = (Args)input;
            maxItem = args.ItemNumbers;
            for (int i = 0; i < maxItem; i++)
            {
                Empty.WaitOne();
                Thread.Sleep(500);
                Access.WaitOne();

                string item = storage[0];
                storage.RemoveAt(0);

                Full.Release();
                Access.Release();

                Console.WriteLine(" - Took " + item + " by " + args.Thread.Name);
            }
        }
    }
}