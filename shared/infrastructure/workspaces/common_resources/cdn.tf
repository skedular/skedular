locals {
  s3_origin_id = "skedular-cdn-${var.environment}"
  domain_name  = local.is_staging ? "awscdnstaging.${module.common.cloudflare_webapp_domain_name}" : "awscdn.${module.common.cloudflare_webapp_domain_name}"
}

data "aws_caller_identity" "current" {}

resource "aws_s3_bucket" "s3_cdn_bucket" {
  bucket = "skedular-cdn-${var.environment}"
  tags   = local.tags
}

resource "aws_s3_bucket_public_access_block" "s3_cdn_bucket" {
  bucket = aws_s3_bucket.s3_cdn_bucket.id

  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

resource "aws_acm_certificate" "cdn_acm_certificate" {
  domain_name       = local.domain_name
  validation_method = "DNS"

  lifecycle {
    create_before_destroy = true
  }
}

resource "cloudflare_dns_record" "aws_cdn_certification_validation" {
  zone_id = module.common.cloudflare_webapp_zone_id

  name    = tolist(aws_acm_certificate.cdn_acm_certificate.domain_validation_options)[0].resource_record_name
  content = tolist(aws_acm_certificate.cdn_acm_certificate.domain_validation_options)[0].resource_record_value
  type    = tolist(aws_acm_certificate.cdn_acm_certificate.domain_validation_options)[0].resource_record_type

  proxied = false
  ttl     = 600
}

resource "aws_cloudfront_origin_access_control" "s3_cdn" {
  name                              = "s3_cdn"
  description                       = "S3 CDN"
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

resource "aws_cloudfront_distribution" "s3_cdn_distribution" {
  origin {
    domain_name              = aws_s3_bucket.s3_cdn_bucket.bucket_regional_domain_name
    origin_access_control_id = aws_cloudfront_origin_access_control.s3_cdn.id
    origin_id                = local.s3_origin_id
  }

  enabled         = true
  is_ipv6_enabled = true

  default_cache_behavior {
    allowed_methods  = ["GET", "HEAD"]
    cached_methods   = ["GET", "HEAD"]
    target_origin_id = local.s3_origin_id

    cache_policy_id          = "658327ea-f89d-4fab-a63d-7e88639e58f6" # CachingOptimized
    origin_request_policy_id = "88a5eaf4-2fd4-4709-b370-b4c650ea3fcf" # CORS-S3Origin

    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 0
    default_ttl            = 3600
    max_ttl                = 86400
  }

  price_class = "PriceClass_All"

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = true
  }

  tags = local.tags
}

resource "aws_s3_bucket_policy" "cloudfront_access" {
  bucket = aws_s3_bucket.s3_cdn_bucket.id

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Principal = {
          Service = "cloudfront.amazonaws.com"
        },
        Action   = "s3:GetObject",
        Resource = "${aws_s3_bucket.s3_cdn_bucket.arn}/*",
        Condition = {
          StringEquals = {
            "AWS:SourceArn" = "arn:aws:cloudfront::${data.aws_caller_identity.current.account_id}:distribution/${aws_cloudfront_distribution.s3_cdn_distribution.id}"
          }
        }
      }
    ]
  })
}
