namespace OBSCaster {
    abstract class OutputHandler {

        public abstract void dispatchEvent(ConsoleEvent type, int value = -1);

        public abstract bool connect(string ip, int port, string password);

        public abstract void disconnect();
    }
}
