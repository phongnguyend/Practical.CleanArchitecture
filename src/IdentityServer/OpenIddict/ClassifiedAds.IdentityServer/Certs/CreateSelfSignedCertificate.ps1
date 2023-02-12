$date_now = Get-Date
$extended_date = $date_now.AddYears(10)
$cert = New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname classifiedads.identityserver -notafter $extended_date
$pwd = ConvertTo-SecureString -String 'password1234' -Force -AsPlainText
$path = 'cert:\localMachine\my\' + $cert.thumbprint
Export-PfxCertificate -cert $path -FilePath classifiedads.identityserver.pfx -Password $pwd