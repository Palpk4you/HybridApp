using Autofac;

public class ServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(UserService).Assembly)
               .Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
        
        
        /*using Microsoft.AspNetCore.Identity.UI.Services;

        // Explicitly register EmailService for Identity
        builder.RegisterType<EmailService>()
               .As<IEmailService>()   // your own interface
               .As<IEmailSender>()    // Identity’s interface
               .InstancePerLifetimeScope();
        */

    }
}