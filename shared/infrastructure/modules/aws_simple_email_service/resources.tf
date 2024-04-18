resource "aws_ses_domain_identity" "default" {
  domain = var.domain
}

resource "aws_ses_domain_dkim" "default" {
  domain = aws_ses_domain_identity.default.domain
}

data "cloudflare_zone" "default" {
  name = var.cloudflare_domain
}

resource "cloudflare_record" "default" {
  count   = 3
  zone_id = data.cloudflare_zone.default.id
  name    = "${aws_ses_domain_dkim.default.dkim_tokens[count.index]}._domainkey.${aws_ses_domain_identity.default.domain}"
  value   = "${aws_ses_domain_dkim.default.dkim_tokens[count.index]}.dkim.amazonses.com"
  type    = "CNAME"
  proxied = false
  ttl     = 600
}
