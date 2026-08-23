import { DeleteIcon, TickIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
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
import Chip from '@mui/material/Chip';
import Grid from '@mui/material/Grid';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import {
  BodyIconTypography,
  coal,
  defaultButtonStyle,
  emerald,
  ExtraLargeHeadingIconTypography,
  LeadIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import CreditCard from '@skedular/ui/commons/credit-card';
import { memo, useContext, useEffect, useState } from 'react';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
};

const isContactUsOffering = (offering: { code?: string | null; isEnterprise: boolean }) => offering.isEnterprise || offering.code === 'ENTERPRISE_CUSTOM_V1';
const offeringOrder = ['FREE_TIER_V1', 'PAY_AS_YOU_GO_V1', 'ENTERPRISE_CUSTOM_V1'];
const getOfferingOrder = (code?: string | null) => (offeringOrder.indexOf(code ?? '') === -1 ? offeringOrder.length : offeringOrder.indexOf(code ?? ''));

type InnerProps = {
  organizationCustomDomain: string;
  onReloadRequired: () => void;
  queryReference: PreloadedQuery<organizationAdminSubscriptionsSectionQuery>;
};

const getOfferingPriceLabel = (offering: { fixedPrice: number | null | undefined; unitPrice: number | null | undefined; isEnterprise: boolean }) =>
  offering.isEnterprise ? 'TBC' : ((offering.fixedPrice ?? offering.unitPrice ?? 0) / 100).toFixed(0);

const getCurrencyLabel = (currency: { readonly name: string } | null | undefined) => currency?.name ?? '';

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
        code
        canCancel
        isEnterprise
        name
        start
        end
        unitPrice
        fixedPrice
        currency {
          name
        }
        featureSet
        underPriceLines
        free
      }
      availableOfferings {
        isEnterprise
        code
        name
        unitPrice
        fixedPrice
        currency {
          name
        }
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
  const [commitUpdateOrganizationOfferingPatch] = useMutation<organizationAdminSubscriptionsSection_updateOrganizationOfferingMutation>(graphql`
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
    commitRemoveOrganizationPaymentMethod({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't remove that payment method. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't remove that payment method. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleCancelActiveOfferingClick = () => {
    const name = organization.name;

    commitCancelOrganizationOffering({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't cancel the active plan for organization '${name}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          onReloadRequired();
          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't cancel the active plan for organization '${name}'. ${error.message}`} />, errorNotificationOptions);
        onReloadRequired();
      },
    });
  };

  const handleUpgradeOfferingClick = (code: string) => {
    const name = organization.name;

    commitUpdateOrganizationOfferingPatch({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
          fieldsToUpdate: ['OFFERING_CODE'],
          offeringCode: code,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't update the active plan for organization '${name}'. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          onReloadRequired();
          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't update the active plan for organization '${name}'. ${error.message}`} />, errorNotificationOptions);
        onReloadRequired();
      },
    });
  };

  return (
    <>
      <Box sx={{ width: '100%' }}>
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', md: 'repeat(auto-fit, minmax(240px, 1fr))' }, gap: 2, justifyContent: 'flex-start' }}>
          {activeOffering && (
            <Grid sx={{ order: getOfferingOrder(activeOffering.code) }}>
              <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%', backgroundColor: 'white', display: 'grid', gridTemplateRows: '1fr auto' }}>
                <CardContent sx={{ marginLeft: 1, display: 'grid', gridTemplateRows: 'auto auto 1fr', paddingBottom: 2 }}>
                  <StackRow spacing={1} sx={{ alignItems: 'center' }}>
                    <LeadIconTypography label={activeOffering.name} sx={{ color: coal }} />
                    <Chip label="Current plan" size="small" color="success" />
                  </StackRow>
                  <StackRow spacing={1} sx={{ marginTop: -2, minHeight: 72, alignItems: 'flex-start' }}>
                    <ExtraLargeHeadingIconTypography label={getOfferingPriceLabel(activeOffering)} sx={{ paddingTop: 4, color: coal }} />
                    <BodyIconTypography label={getCurrencyLabel(activeOffering.currency)} sx={{ color: coal, paddingTop: 5 }} />
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
                </CardContent>
                <CardActions sx={{ justifyContent: 'flex-start', px: 2, minHeight: 52, marginTop: 'auto' }}>
                  {!activeOffering.free && activeOffering.canCancel && (
                    <Button color="secondary" variant="contained" onClick={handleCancelActiveOfferingClick} sx={defaultButtonStyle}>
                      Cancel
                    </Button>
                  )}
                </CardActions>
              </Card>
            </Grid>
          )}

          {availableOfferings.map((availableOffering) => (
            <Grid key={availableOffering.code} sx={{ order: getOfferingOrder(availableOffering.code) }}>
              <Card sx={{ width: { xs: '100%', sm: 300 }, height: '100%', backgroundColor: 'white', display: 'grid', gridTemplateRows: '1fr auto' }}>
                <CardContent sx={{ marginLeft: 1, display: 'grid', gridTemplateRows: 'auto auto 1fr', paddingBottom: 2 }}>
                  <LeadIconTypography label={availableOffering.name} sx={{ color: coal }} />
                  <StackRow spacing={1} sx={{ marginTop: -2, minHeight: 72, alignItems: 'flex-start' }}>
                    <ExtraLargeHeadingIconTypography label={getOfferingPriceLabel(availableOffering)} sx={{ paddingTop: 4, color: coal }} />
                    <BodyIconTypography label={getCurrencyLabel(availableOffering.currency)} sx={{ color: coal, paddingTop: 5 }} />
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
                  </List>
                </CardContent>

                <CardActions sx={{ justifyContent: 'flex-start', px: 2, minHeight: 52, marginTop: 'auto' }}>
                  {organization.hasAttachedPaymentMethod && !isContactUsOffering(availableOffering) && (
                    <Button color="primary" variant="contained" onClick={() => handleUpgradeOfferingClick(availableOffering.code)} sx={{ textTransform: 'none', color: 'white' }}>
                      Upgrade
                    </Button>
                  )}

                  {isContactUsOffering(availableOffering) && (
                    <Button href="mailto:support@getskedular.com" variant="contained" sx={{ textTransform: 'none', backgroundColor: 'black', color: 'white' }}>
                      Contact Us
                    </Button>
                  )}
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Box>

        {!organization.hasAttachedPaymentMethod && availableOfferings.some((offering) => !offering.free && !isContactUsOffering(offering)) && (
          <Box sx={{ pt: 2 }}>
            <SmallIconTypography label="A payment method is required to upgrade to a paid plan." sx={{ color: coal }} />
          </Box>
        )}

        <Box sx={{ mt: 3, pt: 2, borderTop: 1, borderColor: 'divider' }}>
          <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
            <LeadIconTypography label="Payment method" sx={{ color: coal }} />
            {!organization.hasAttachedPaymentMethod && availableOfferings.some((offering) => !offering.free && !isContactUsOffering(offering)) && (
              <Button variant="contained" onClick={handleAddPaymentMethodClicked} sx={{ textTransform: 'none', color: 'white' }}>
                Add Payment Method
              </Button>
            )}
          </StackRow>
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

          {organization.paymentMethods.length === 0 && <Box sx={{ pt: 1 }} />}
        </Box>
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
