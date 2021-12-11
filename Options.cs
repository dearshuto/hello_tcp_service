using CommandLine;

namespace hello_tcp_service
{
    enum ServiceType
    {
        Server,
        Client,
    }

    class Options
    {
        // -a と -aaa の二つ指定可能
        [Option('p', "port", Required = true, HelpText = "")]
        public int Port { get; set; }

        // enumもいける（Hoge, Fuga などと指定する）
        [Option('s', "service-type", Required = true, HelpText = "")]
        public ServiceType ServiceType { get; set; }

        // 上記指定以外のオプションや文字列が入る
        [Option('c', "request-close-server", HelpText = "")]
        public bool IsServerShutdownRequested { get; set; }
    }
}