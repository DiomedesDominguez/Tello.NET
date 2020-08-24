using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tello.NET.Models;

namespace Tello.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,
                              IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        IActionResult Manual(string commands)
        {
            var UDPClientPortort = _configuration.GetSection("TelloSettings").GetValue<int>("UDPClientPort");
            var UDPClientIp = _configuration.GetSection("TelloSettings").GetValue<string>("UDPClientIp");
            var UDPServerIp = _configuration.GetSection("TelloSettings").GetValue<string>("UDPServerIp");
            var udpClient = new UdpClient(UDPClientPortort);
            udpClient.Connect(IPAddress.Parse(UDPClientIp), UDPClientPortort);

            foreach (var command in commands.Split(Environment.NewLine))
            {
                var sendBytes = Encoding.ASCII.GetBytes(command);
                udpClient.Send(sendBytes, sendBytes.Length);

                var RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(UDPServerIp), 0);

                var receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
            }

            udpClient.Close();

            return null;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
