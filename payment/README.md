# Stripe forward command

```shell
stripe listen -l --forward-to http://0.0.0.0:10100/payment/api/v1/stripe/platform/account/webhook --forward-connect-to http://0.0.0.0:10100/payment/api/v1/stripe/connect/account/webhook
```
