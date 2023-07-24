namespace virtual_disk
{
    internal class Input
    {
        public static List<string> GetCmdCommand()
        {
            string cmdCommandInput = Console.ReadLine();
            string[] cmdCommandWords = cmdCommandInput.Split(' ');
            //if (cmdCommandWords.Length == 2 && cmdCommandWords[1] == "")
            //    cmdCommandWords = cmdCommandWords.Take(cmdCommandWords.Length - 1).ToArray();
            //cmd CommandWords has spaces, remove them.
            List<string> cmdCommand = new List<string>();

            for(int i = 0; i < cmdCommandWords.Length; i++)
            {
                if (cmdCommandWords[i] != "")
                    cmdCommand.Add(cmdCommandWords[i]);
            }
            return cmdCommand;
        }
    }
}