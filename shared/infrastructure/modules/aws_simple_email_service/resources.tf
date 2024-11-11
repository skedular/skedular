resource "aws_ses_domain_identity" "default" {
  domain = var.domain
}

resource "aws_ses_domain_dkim" "default" {
  domain = aws_ses_domain_identity.default.domain
}

resource "cloudflare_record" "dkim" {
  count   = 3
  zone_id = data.cloudflare_zone.default.id
  name    = "${aws_ses_domain_dkim.default.dkim_tokens[count.index]}._domainkey.${aws_ses_domain_identity.default.domain}"
  content = "${aws_ses_domain_dkim.default.dkim_tokens[count.index]}.dkim.amazonses.com"
  type    = "CNAME"
  proxied = false
  ttl     = 600
}

data "cloudflare_zone" "default" {
  name = var.cloudflare_domain
}

resource "cloudflare_record" "dmarc" {
  zone_id = data.cloudflare_zone.default.id
  name    = "_dmarc.${aws_ses_domain_identity.default.domain}"
  content = "\"v=DMARC1; p=none;\""
  type    = "TXT"
  proxied = false
  ttl     = 600
}
