resource "aws_ses_domain_identity" "default" {
  domain = var.domain
}

resource "aws_ses_domain_dkim" "default" {
  domain = aws_ses_domain_identity.default.domain
}

resource "cloudflare_dns_record" "dkim" {
  count   = 3
  zone_id = var.cloudflare_zone_id
  name    = "${aws_ses_domain_dkim.default.dkim_tokens[count.index]}._domainkey.${aws_ses_domain_identity.default.domain}"
  content = "${aws_ses_domain_dkim.default.dkim_tokens[count.index]}.dkim.amazonses.com"
  type    = "CNAME"
  proxied = false
  ttl     = 600
}

resource "cloudflare_dns_record" "dmarc" {
  zone_id = var.cloudflare_zone_id
  name    = "_dmarc.${aws_ses_domain_identity.default.domain}"
  content = "\"v=DMARC1; p=none; rua=mailto:dmarc-reports@${var.cloudflare_domain}; ruf=mailto:dmarc-failures@${var.cloudflare_domain}; aspf=r; sp=none;\""
  type    = "TXT"
  proxied = false
  ttl     = 600
}
