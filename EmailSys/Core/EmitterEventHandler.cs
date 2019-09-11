
namespace EmailSys.Core
{

    public delegate void EmitterSuccessEventHandler(object sender, EmitterSuccessEventArgs args);

    public delegate void EmitterSmtpErrorEventHandler(object sender,EmitterSmtpErrorEventAgs args);

    public delegate void EmitterReleasEventHandler(object sender,EmitterReleasEventArgs[] args);

    public delegate void EmitterErrorEventHandler(object sender,EmitterErrorEventArgs args);

    public delegate void EmitterArgErrorEventHandler(object sender, EmitterArgErrorEventArgs args);
}
