import { DeleteIcon, ErrorIcon, NewIcon, TickIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { AddOrganizationPaymentMethodDialog } from '@/components/organization/addOrganizationPaymentMethod';
import type { organizationAdminSubscriptionsSectionQuery } from '@/queries/__generated__/organizationAdminSubscriptionsSectionQuery.graphql';
import type { organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation } from '@/queries/__generated__/organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation.graphql';
import type { organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation } from '@/queries/__generated__/organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation.graphql';
import type { organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation } from '@/queries/__generated__/organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Grid from '@mui/material/Grid';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import {
  BodyIconTypography,
  coal,
  CreditCard,
  defaultButtonStyle,
  emerald,
  ExtraLargeHeadingIconTypography,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import { memo, useContext, useEffect, useState } from 'react';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

type InnerProps = {
  organizationCustomDomain: string;
  onReloadRequired: () => void;
  queryReference: PreloadedQuery<organizationAdminSubscriptionsSectionQuery>;
};

const RootQuery = graphql`
  query organizationAdminSubscriptionsSectionQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
      hasAttachedPaymentMethod
      paymentMethods {
        id
        cardBrand
        cardExpiryMonth
        cardExpiryYear
        cardLastFourDigit
      }
      activeOffering {
        id
        isEnterprise
        name
        start
        end
        unitPrice
        featureSet
        underPriceLines
        free
      }
      availableOfferings {
        isEnterprise
        code
        name
        unitPrice
        featureSet
        underPriceLines
        free
      }
    }
  }
`;

const OrganizationAdminSubscriptionsSectionContent = ({ organizationCustomDomain, onReloadRequired, queryReference }: InnerProps) => {
  const rootData = usePreloadedQuery<organizationAdminSubscriptionsSectionQuery>(RootQuery, queryReference);
  const [commitCancelOrganizationOffering] = useMutation<organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation>(graphql`
    mutation organizationAdminSubscriptionsSection_cancelOrganizationOfferingMutation($input: CancelOrganizationOfferingInput!) {
      cancelOrganizationOffering(input: $input) {
        clientMutationId
      }
    }
  `);
  const [commitUpdateOrganizationOffering] = useMutation<organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation>(graphql`
    mutation organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation($input: UpdateOrganizationOfferingInput!) {
      updateOrganizationOffering(input: $input) {
        clientMutationId
      }
    }
  `);
  const [commitRemoveOrganizationPaymentMethod] = useMutation<organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation>(graphql`
    mutation organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation($input: RemoveOrganizationPaymentMethodInput!) {
      removeOrganizationPaymentMethod(input: $input) {
        clientMutationId
      }
    }
  `);

  const organization = rootData.organization;
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [isAddPaymentMethodDialogOpen, setIsAddPaymentMethodDialogOpen] = useState(false);

  if (!organization) {
    return null;
  }

  const activeOffering = organization.activeOffering;
  const availableOfferings = organization.availableOfferings ?? [];

  const handleAddPaymentMethodClicked = () => {
    setIsAddPaymentMethodDialogOpen(true);
  };

  const handleAddPaymentMethodCancel = () => {
    setIsAddPaymentMethodDialogOpen(false);
    onReloadRequired();
  };

  const handleRemovePaymentMethodClick = (id: string) => {
    const toastId = themedToast(<NotificationContent content="Removing payment method..." />, infoNotificationOptions);

    commitRemoveOrganizationPaymentMethod({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't remove that payment method. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content="The payment method has been removed." />,
        });
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't remove that payment method. ${error.message}`} />,
        });
      },
    });
  };

  const handleCancelActiveOfferingClick = () => {
    const name = organization.name;
    const toastId = themedToast(<NotificationContent content={`Cancelling the active plan for organization '${name}'...`} />, infoNotificationOptions);

    commitCancelOrganizationOffering({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't cancel the active plan for organization '${name}'. ${getRelayErrorMessage(errors)}`} />,
          });
          onReloadRequired();
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`The active plan for organization '${name}' has been cancelled.`} />,
        });
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't cancel the active plan for organization '${name}'. ${error.message}`} />,
        });
        onReloadRequired();
      },
    });
  };

  const handleUpgradeOfferingClick = (code: string) => {
    const name = organization.name;
    const toastId = themedToast(<NotificationContent content={`Updating the active plan for organization '${name}'...`} />, infoNotificationOptions);

    commitUpdateOrganizationOffering({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
          offeringCode: code,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update the active plan for organization '${name}'. ${getRelayErrorMessage(errors)}`} />,
          });
          onReloadRequired();
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`The active plan for organization '${name}' has been updated.`} />,
        });
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't update the active plan for organization '${name}'. ${error.message}`} />,
        });
        onReloadRequired();
      },
    });
  };

  return (
    <>
      <Box sx={{ pb: 2 }}>
        <SettingsSectionCard title="Subscriptions" description="Review the active plan and the available upgrades for the organization.">
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', md: 'repeat(auto-fit, minmax(280px, 320px))' }, gap: 2, justifyContent: 'center' }}>
            {activeOffering && (
              <Grid>
                <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%', backgroundColor: 'white' }}>
                  <CardContent sx={{ marginLeft: 1 }}>
                    <BodyIconTypography label={activeOffering.name} sx={{ color: coal }} />
                    <StackRow spacing={0.5} sx={{ marginTop: -2 }}>
                      <ExtraLargeHeadingIconTypography label={(activeOffering.unitPrice / 100).toFixed(0)} sx={{ paddingTop: 4, color: coal }} />
                      <BodyIconTypography label="$" sx={{ color: coal }} />
                    </StackRow>

                    <List sx={{ padding: 0 }}>
                      <Box sx={{ marginTop: 2, marginBottom: 4 }}>
                        {activeOffering.underPriceLines.map((item, index) => (
                          <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                            <ListItemText>
                              <SmallIconTypography label={item} sx={{ color: coal }} />
                            </ListItemText>
                          </ListItem>
                        ))}
                      </Box>

                      {activeOffering.featureSet.map((item, index) => (
                        <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                          <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                            <TickIcon fontSize="small" sx={{ color: activeOffering.isEnterprise ? coal : emerald }} />
                          </ListItemIcon>
                          <ListItemText>
                            <SmallIconTypography label={item} sx={{ color: coal }} />
                          </ListItemText>
                        </ListItem>
                      ))}
                    </List>

                    <CardActions sx={{ justifyContent: 'center' }}>
                      {!activeOffering.free && (
                        <Button color="secondary" variant="contained" onClick={handleCancelActiveOfferingClick} sx={defaultButtonStyle}>
                          Cancel
                        </Button>
                      )}
                    </CardActions>
                  </CardContent>
                </Card>
              </Grid>
            )}

            {availableOfferings.map((availableOffering) => (
              <Grid key={availableOffering.code}>
                <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%', backgroundColor: 'white' }}>
                  <CardContent sx={{ marginLeft: 1 }}>
                    <BodyIconTypography label={availableOffering.name} sx={{ color: coal }} />
                    <StackRow spacing={0.5} sx={{ marginTop: -2 }}>
                      {availableOffering.unitPrice > 0 && (
                        <ExtraLargeHeadingIconTypography label={(availableOffering.unitPrice / 100).toFixed(0)} sx={{ paddingTop: 4, color: coal }} />
                      )}
                      {availableOffering.isEnterprise && <ExtraLargeHeadingIconTypography label="TBC" sx={{ paddingTop: 4, color: coal }} />}
                      <BodyIconTypography label="$" sx={{ color: coal }} />
                    </StackRow>

                    <List sx={{ padding: 0 }}>
                      <Box sx={{ marginTop: 2, marginBottom: 4 }}>
                        {availableOffering.underPriceLines.map((item, index) => (
                          <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                            <ListItemText>
                              <SmallIconTypography label={item} sx={{ color: coal }} />
                            </ListItemText>
                          </ListItem>
                        ))}
                      </Box>

                      {availableOffering.featureSet.map((item, index) => (
                        <ListItem key={index} alignItems="flex-start" sx={{ padding: 0 }}>
                          <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                            <TickIcon fontSize="small" sx={{ color: availableOffering.isEnterprise ? coal : emerald }} />
                          </ListItemIcon>
                          <ListItemText>
                            <SmallIconTypography label={item} sx={{ color: coal }} />
                          </ListItemText>
                        </ListItem>
                      ))}

                      {!organization.hasAttachedPaymentMethod && (
                        <ListItem alignItems="flex-start" sx={{ padding: 0, paddingTop: 1 }}>
                          <ListItemIcon sx={{ minWidth: 'auto', marginRight: 1 }}>
                            <ErrorIcon fontSize="large" sx={{ color: 'red' }} />
                          </ListItemIcon>
                          <ListItemText>
                            <SmallIconTypography label="You need to have payment method setup in order to upgrade to this offering." color="red" />
                          </ListItemText>
                        </ListItem>
                      )}
                    </List>
                  </CardContent>

                  <CardActions sx={{ justifyContent: 'center' }}>
                    {!organization.hasAttachedPaymentMethod && (
                      <Button variant="contained" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none', color: 'white' }}>
                        Add Payment Method
                      </Button>
                    )}

                    {organization.hasAttachedPaymentMethod && !availableOffering.isEnterprise && (
                      <Button color="primary" variant="contained" onClick={() => handleUpgradeOfferingClick(availableOffering.code)} sx={{ textTransform: 'none', color: 'white' }}>
                        Upgrade
                      </Button>
                    )}

                    {organization.hasAttachedPaymentMethod && availableOffering.isEnterprise && (
                      <Button href="mailto:support@getskedular.com" variant="contained" sx={{ textTransform: 'none', backgroundColor: 'black', color: 'white' }}>
                        Contact Us
                      </Button>
                    )}
                  </CardActions>
                </Card>
              </Grid>
            ))}
          </Box>

          {organization.paymentMethods.length > 0 && (
            <StackRow sx={{ gap: 2, flexWrap: 'wrap', pt: 2 }}>
              {organization.paymentMethods.map((item) => (
                <StackColumn key={item.id}>
                  <CreditCard lastFourDigits={item.cardLastFourDigit} expiryDate={`${item.cardExpiryMonth}/${item.cardExpiryYear}`} cardBrand={item.cardBrand} />
                  <Button variant="contained" color="warning" onClick={() => handleRemovePaymentMethodClick(item.id)}>
                    <BodyIconTypography label="Remove Payment Method" invertDefaultColor={paletteMode === 'dark'} startElement={<DeleteIcon />} />
                  </Button>
                </StackColumn>
              ))}
            </StackRow>
          )}

          {organization.paymentMethods.length === 0 && (
            <Box sx={{ pt: 2 }}>
              <Button variant="text" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none' }}>
                <BodyIconTypography label="Add Payment Method" endElement={<NewIcon fontSize="large" />} />
              </Button>
            </Box>
          )}
        </SettingsSectionCard>
      </Box>

      {isAddPaymentMethodDialogOpen && (
        <AddOrganizationPaymentMethodDialog
          organizationCustomDomain={organizationCustomDomain}
          isDialogOpen={isAddPaymentMethodDialogOpen}
          onCancel={handleAddPaymentMethodCancel}
        />
      )}
    </>
  );
};

const OrganizationAdminSubscriptionsSection = ({ organizationCustomDomain }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationAdminSubscriptionsSectionQuery>(RootQuery);
  const [reloadKey, setReloadKey] = useState(uuid());

  useEffect(() => {
    loadQuery(
      { organizationCustomDomain },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain, reloadKey]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <OrganizationAdminSubscriptionsSectionContent
      organizationCustomDomain={organizationCustomDomain}
      onReloadRequired={() => setReloadKey(uuid())}
      queryReference={queryReference}
    />
  );
};

export default memo(OrganizationAdminSubscriptionsSection);
