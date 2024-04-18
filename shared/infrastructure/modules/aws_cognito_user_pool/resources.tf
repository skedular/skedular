resource "aws_cognito_user_pool" "default" {
  name = var.name

  deletion_protection = "ACTIVE"

  alias_attributes = ["email", "preferred_username"]

  user_attribute_update_settings {
    attributes_require_verification_before_update = ["email"]
  }

  email_configuration {
    email_sending_account  = "DEVELOPER"
    from_email_address     = var.from_email_address
    reply_to_email_address = var.reply_to_email_address
    source_arn             = var.simple_email_service_arn
  }

  mfa_configuration = "OFF"

  username_configuration {
    case_sensitive = false
  }

  account_recovery_setting {
    recovery_mechanism {
      name     = "verified_email"
      priority = 1
    }
  }

  password_policy {
    minimum_length                   = 8
    require_lowercase                = true
    require_numbers                  = true
    require_symbols                  = true
    require_uppercase                = true
    temporary_password_validity_days = 7
  }

  auto_verified_attributes = ["email"]

  schema {
    name                     = "sub"
    attribute_data_type      = "String"
    developer_only_attribute = false
    mutable                  = false
    required                 = true

    string_attribute_constraints {
      max_length = "2048"
      min_length = "1"
    }
  }

  schema {
    name                = "email"
    attribute_data_type = "String"
    required            = true
    mutable             = true

    string_attribute_constraints {
      max_length = "2048"
      min_length = "1"
    }
  }

  schema {
    name                = "given_name"
    attribute_data_type = "String"
    required            = false
    mutable             = true

    string_attribute_constraints {
      max_length = "2048"
      min_length = "1"
    }
  }

  schema {
    name                = "family_name"
    attribute_data_type = "String"
    required            = false
    mutable             = true

    string_attribute_constraints {
      max_length = "2048"
      min_length = "1"
    }
  }

  tags = local.tags
}

resource "aws_cognito_identity_provider" "google_provider" {
  user_pool_id  = aws_cognito_user_pool.default.id
  provider_name = var.google_provider_name
  provider_type = "Google"

  provider_details = {
    authorize_scopes = "email profile openid"
    client_id        = var.gcp_unityhub_web_credentials_client_id
    client_secret    = var.gcp_unityhub_web_credentials_client_secret
  }

  attribute_mapping = {
    email          = "email"
    username       = "sub"
    given_name     = "given_name"
    family_name    = "family_name"
    email_verified = "email_verified"
    gender         = "genders"
    name           = "names"
    picture        = "picture"
    phone_number   = "phoneNumbers"
  }
}

resource "aws_cognito_user_pool_domain" "default" {
  domain       = var.domain
  user_pool_id = aws_cognito_user_pool.default.id
}
