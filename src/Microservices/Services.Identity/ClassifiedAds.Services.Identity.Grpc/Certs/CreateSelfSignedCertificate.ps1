$date_now = Get-Date
$extended_date = $date_now.AddYears(3)
$cert = New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname classifiedads.grpc -notafter $extended_date
$pwd = ConvertTo-SecureString -String 'password1234' -Force -AsPlainText
$path = 'cert:\localMachine\my\' + $cert.thumbprint
Export-PfxCertificate -cert $path -FilePath classifiedads.grpc.pfx -Password $pwd