import graphql from 'babel-plugin-relay/macro';
import { memo, useMemo } from 'react';
import { useFragment } from 'react-relay';
import type { notificationCard_NotificationDetails$key } from './__generated__/notificationCard_NotificationDetails.graphql';
import InvitationToJoinLocationNotificationCard from './invitation-to-join-location-notification-card';
import InvitationToJoinOrganizationNotificationCard from './invitation-to-join-organization-notification-card';
import InvitationToJoinTeamNotificationCard from './invitation-to-join-team-notification-card';

type Props = {
  notificationDetailsRelay: notificationCard_NotificationDetails$key;
};

const NotificationCard = ({ notificationDetailsRelay }: Props) => {
  const notificationDetails = useFragment(
    graphql`
      fragment notificationCard_NotificationDetails on Notification {
        id
        notificationType
        ...invitationToJoinOrganizationNotificationCard_NotificationDetails
        ...invitationToJoinLocationNotificationCard_NotificationDetails
        ...invitationToJoinTeamNotificationCard_NotificationDetails
      }
    `,
    notificationDetailsRelay,
  );

  const notificationType = useMemo(() => notificationDetails?.notificationType, [notificationDetails?.notificationType]);

  switch (notificationType) {
    case 'InvitationToJoinOrganization':
      return <InvitationToJoinOrganizationNotificationCard notificationDetailsRelay={notificationDetails} />;

    case 'InvitationToJoinLocation':
      return <InvitationToJoinLocationNotificationCard notificationDetailsRelay={notificationDetails} />;

    case 'InvitationToJoinTeam':
      return <InvitationToJoinTeamNotificationCard notificationDetailsRelay={notificationDetails} />;

    default:
      return <></>;
  }
};

export default memo(NotificationCard);
