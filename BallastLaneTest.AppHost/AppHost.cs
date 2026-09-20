var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.BallastLaneTest_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithHttpEndpoint(port: 5000)
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
