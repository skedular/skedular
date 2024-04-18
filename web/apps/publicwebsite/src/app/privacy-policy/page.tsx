'use client';

import type { Line } from '@repo/shared/components/document';
import { Document, LineType } from '@repo/shared/components/document';
import { PublicMainRootLayout } from '@/components/layouts';
import { memo } from 'react';

const lines: Line[] = [
  {
    lineType: LineType.SingleLine,
    line: 'Privacy Policy',
    variant: 'h1',
    breakLineCount: 2,
  },
  {
    lineType: LineType.SingleLine,
    line: 'UnityHub Privacy Policy',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Welcome to our privacy policy. We respect your privacy and are committed to protecting your personal data. This privacy policy lets you know how we look after your personal data when you visit our website (regardless of where you visit it from) or access our services, and tells you about your privacy rights and how the law protects you.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: '1. Important information and who we are',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Purpose of this privacy policy',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'This privacy policy aims to give you information on how UnityHub collects and processes your personal data through your use of this website and/or our Slack integration, including any data you may provide when accessing or using our services.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Our website and services are not intended for children and we do not knowingly collect data relating to children.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'It is important that you read this privacy policy together with any other privacy policy or fair processing policy we may provide on specific occasions when we are collecting or processing personal data about you so that you are fully aware of how and why we are using your data. This privacy policy supplements other notices and privacy policies and is not intended to override them',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Controller',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'UnityHub Limited, trading as UnityHub, is the data controller and is responsible for your personal data(collectively referred to as “UnityHub”, "we", "us" or "our" in this privacy policy).',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'If you have any questions about this privacy policy, including any requests to exercise your legal rights, please contact us using the details set out below.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Full name of legal entity: UnityHub Limited, trading as UnityHub.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Email address: privacypolicy@unityhub.io',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Postal address:',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'You have the right to make a complaint at any time to your local data protection authority. We would, however, appreciate the chance to deal with your concerns before you approach the data protection authority so please contact us in the first instance.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Changes to the privacy policy and your duty to inform us of changes',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We keep our privacy policy under regular review.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'It is important that the personal data we hold about you is accurate and current. Please keep us informed if your personal data changes during your relationship with us.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Third-party links',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'This website may include links to third-party websites, plug-ins, and applications. Clicking on those links or enabling those connections may allow third parties to collect or share data about you. We do not control these third-party websites and are not responsible for their privacy statements. When you leave our website, we encourage you to read the privacy policy of every website you visit.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: '2. The data we collect about you',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Personal data, or personal information, means any information about an individual from which that person can be identified. It does not include data where the identity has been removed (anonymous data).',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We may collect, use, store and transfer different kinds of personal data about you which we have grouped together as follows:',
    variant: 'body1',
  },
  {
    lineType: LineType.BulletPoint,
    bulletPointLines: [
      {
        lineType: LineType.SingleLine,
        line: 'Identity and Contact Data includes name, username (including Slack username) or similar identifier and email address.',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'Technical Data includes internet protocol (IP) address, your login data, browser type and version, time zone setting and location, browser plug-in types and versions, operating system and platform, and other technology on the devices you use to access this website or our services.',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'Usage Data includes information about how you use our website and services.',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'Marketing and Communications Data includes your preferences in receiving marketing from us and your communication preferences.',
        variant: 'body1',
      },
    ],
  },
  {
    lineType: LineType.SingleLine,
    line: 'We also collect, use and share Aggregated Data such as statistical or demographic data for any purpose. Aggregated Data could be derived from your personal data but is not considered personal data in law as this data will not directly or indirectly reveal your identity. For example, we may aggregate your Usage Data to calculate the percentage of users accessing a specific website feature. However, if we combine or connect Aggregated Data with your personal data so that it can directly or indirectly identify you, we treat the combined data as personal data which will be used in accordance with this privacy policy.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We do not collect any Special Categories of Personal Data about you (this includes details about your race or ethnicity, religious or philosophical beliefs, sex life, sexual orientation, political opinions, trade union membership, information about your health, and genetic and biometric data). Nor do we collect any information about criminal convictions and offences.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'If you fail to provide personal data',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Where we need to collect personal data by law, or under the terms of a contract we have with you, and you fail to provide that data when requested, we may not be able to perform the contract we have or are trying to enter into with you (for example, to provide you with goods or services). In this case, we may have to cancel a product or service you have with us but we will notify you if this is the case at the time.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: '3. How is your personal data collected?',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We use different methods to collect data from and about you including through:',
    variant: 'body1',
  },
  {
    lineType: LineType.BulletPoint,
    bulletPointLines: [
      {
        lineType: LineType.SingleLine,
        line: 'Direct interactions. You may give us, or we may automatically receive, your Identity and Contact Data when you use our services.',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'Automated technologies or interactions. As you interact with our website and services, we will automatically collect Technical Data about your equipment, browsing actions and patterns. We collect this personal data by using cookies and other similar technologies.',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'Third parties or publicly available sources. We may automatically receive your Identity and Contact Data when the company you work for integrates their Slack application with our services.',
        variant: 'body1',
      },
    ],
  },
  {
    lineType: LineType.SingleLine,
    line: '4. How we use your personal data',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We will only use your personal data when the law allows us to. Most commonly, we will use your personal data in the following circumstances:',
    variant: 'body1',
  },
  {
    lineType: LineType.BulletPoint,
    bulletPointLines: [
      {
        lineType: LineType.SingleLine,
        line: 'Where we need to perform a contract we are about to enter into or have entered into with you.',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'Where it is necessary for our legitimate interests (or those of a third party) and your interests and fundamental rights do not override those interests.',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'Where we need to comply with a legal obligation.',
        variant: 'body1',
      },
    ],
  },
  {
    lineType: LineType.SingleLine,
    line: 'Generally, we do not rely on consent as a legal basis for processing your personal data although, when required by law, we will get your consent before sending direct marketing communications to you via email or text message. You have the right to withdraw consent to marketing at any time by contacting us.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Promotional offers from us',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We may use your Identity, Contact, Technical, Usage and Profile Data to form a view on what we think you may want or need, or what may be of interest to you. This is how we decide which products, services and offers may be relevant for you.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Opting out',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'You can ask us or third parties to stop sending you marketing messages by contacting us at any time.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Change of purpose',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We will only use your personal data for the purposes for which we collected it, unless we reasonably consider that we need to use it for another reason and that reason is compatible with the original purpose. If you wish to get an explanation as to how the processing for the new purpose is compatible with the original purpose, please contact us.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'If we need to use your personal data for an unrelated purpose, we will notify you and we will explain the legal basis which allows us to do so.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Please note that we may process your personal data without your knowledge or consent, in compliance with the above rules, where this is required or permitted by law.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: '5. Disclosures of your personal data',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We may share your personal data with the parties set out below for the purposes set out in the table above.',
    variant: 'body1',
  },
  {
    lineType: LineType.BulletPoint,
    bulletPointLines: [
      {
        lineType: LineType.SingleLine,
        line: 'External service providers that we work with operate our business, website and services.',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'Third parties to whom we may choose to sell, transfer or merge parts of our business or our assets. Alternatively, we may seek to acquire other businesses or merge with them. If a change happens to our business, then the new owners may use your personal data in the same way as set out in this privacy policy.',
        variant: 'body1',
      },
    ],
  },
  {
    lineType: LineType.SingleLine,
    line: 'We require all third parties to respect the security of your personal data and to treat it in accordance with the law. We do not allow our third-party service providers to use your personal data for their own purposes and only permit them to process your personal data for specified purposes and in accordance with our instructions.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'International transfers',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Some of the external service providers to which we need to transfer personal data are based outside the New Zealand. This means that their processing of your personal data will involve a transfer of data outside the New Zealand.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Whenever we transfer your personal data out of the New Zealand, we ensure a similar degree of protection is afforded to it by ensuring that appropriate safeguards are implemented.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: '7. Data security',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We have put in place appropriate security measures to prevent your personal data from being accidentally lost, used or accessed in an un authorised way, altered or disclosed. In addition, we limit access to your personal data to those employees, agents, contractors and other third parties who have a business need to know. They will only process your personal data on our instructions and they are subject to a duty of confidentiality.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We have put in place procedures to deal with any suspected personal data breach and will notify you and any applicable regulator of a breach where we are legally required to do so.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: '8. Data retention',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'How long will you use my personal data for?',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We will only retain your personal data for as long as reasonably necessary to fulfil the purposes we collected it for, including for the purposes of satisfying any legal, regulatory, tax, accounting or reporting requirements. We may retain your personal data for a longer period in the event of a complaint or if we reasonably believe there isa prospect of litigation in respect to our relationship with you.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'To determine the appropriate retention period for personal data, we consider the amount, nature and sensitivity of the personal data, the potential risk of harm from unauthorised use or disclosure of your personal data, the purposes for which we process your personal data and whether we can achieve those purposes through other means,and the applicable legal, regulatory, tax, accounting or other requirements.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: '9. Your legal rights',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Under certain circumstances, you have various rights under data protection laws in relation to your personal data, including the right to:  ',
    variant: 'body1',
  },
  {
    lineType: LineType.BulletPoint,
    bulletPointLines: [
      {
        lineType: LineType.SingleLine,
        line: 'request access to your personal data;',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'request correction of your personal data;',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'request erasure of your personal data;',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'object to processing of your personal data;',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'request restriction of processing your personal data;',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'request transfer of your personal data; and',
        variant: 'body1',
      },
      {
        lineType: LineType.SingleLine,
        line: 'withdraw consent.',
        variant: 'body1',
      },
    ],
  },
  {
    lineType: LineType.SingleLine,
    line: 'If you wish to exercise any of the rights set out above, please contact us.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'No fee usually required',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'You will not have to pay a fee to access your personal data (or to exercise any of the other rights). However, we may charge a reasonable fee if your request is clearly unfounded, repetitive or excessive. Alternatively, we could refuse to comply with your request in these circumstances.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'What we may need from you',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We may need to request specific information from you to help us confirm your identity and ensure your right to access your personal data (or to exercise any of your other rights). This is a security measure to ensure that personal data is not disclosed to any person who has no right to receive it. We may also contact you to ask you for further information in relation to your request to speed up our response.',
    variant: 'body1',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Time limit to respond',
    variant: 'h4',
    breakLineCount: 1,
  },
  {
    lineType: LineType.SingleLine,
    line: 'We try to respond to all legitimate requests within one month. Occasionally it could take us longer than a month if your request is particularly complex or you have made a number of requests. In this case, we will notify you and keep you updated.',
    variant: 'body1',
    breakLineCount: 2,
  },
  {
    lineType: LineType.SingleLine,
    line: 'Updated 8th May 2024.',
    variant: 'body1',
  },
];

const PrivacyPolicy = () => {
  return (
    <PublicMainRootLayout>
      <Document lines={lines} />
    </PublicMainRootLayout>
  );
};

export default memo(PrivacyPolicy);
