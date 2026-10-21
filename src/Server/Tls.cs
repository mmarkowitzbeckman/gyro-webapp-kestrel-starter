using System.Security.Cryptography.X509Certificates;

namespace Gyro;

public static class Tls
{
    public static X509Certificate2 LoadCertificate()
    {
        var keyPath = Config.String("TLS_KEY_PATH", "certs/key.pem");
        var certPath = Config.String("TLS_CERT_PATH", "certs/cert.pem");

        var certificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);

        // SslStream on Windows can't use a PEM-imported ephemeral key; round-trip it through PKCS#12.
        return OperatingSystem.IsWindows()
            ? X509CertificateLoader.LoadPkcs12(certificate.Export(X509ContentType.Pkcs12), null)
            : certificate;
    }
}
