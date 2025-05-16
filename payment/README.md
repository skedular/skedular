# Stripe forward command

```shell
stripe listen -l --forward-to http://0.0.0.0:10100/v1/payment/stripe/platform/account/webhook --forward-connect-to http://0.0.0.0:10100/v1/payment/stripe/connect/account/webhook
```
