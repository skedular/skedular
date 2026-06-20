import type { ReviewStatus } from "./content-types";

export interface LegalPage {
  id: "terms-of-service" | "privacy-policy";
  path: string;
  title: string;
  description: string;
  sourceUrl: string;
  reviewStatus: ReviewStatus;
  lastSourceReview: string;
  heroSummary: string;
  notice: string;
  html: string;
}

export const legalPages: LegalPage[] = [
  {
    id: "terms-of-service",
    path: "/terms-of-service",
    title: "Terms of Service | Skedular",
    description:
      "Review Skedular's Terms of Service for SaaS access, support, customer responsibilities, billing, term, termination, and data processing terms.",
    sourceUrl: "https://getskedular.com/terms-of-service/",
    reviewStatus: "pending",
    lastSourceReview: "2026-06-05",
    heroSummary:
      "These Terms of Service are migrated from the current Skedular legal source and preserve the source legal document wording.",
    notice:
      "Legal document migrated from the current Skedular Terms of Service source. Do not rewrite or summarize this content without business or legal approval.",
    html: String.raw`<h3>SKEDULAR ORDER FORM</h3>
<p>Services: Scheduling employee office days (the “Services”).</p>
<p>Services Fees: fees are calculated in accordance with the plan chosen by the Customer.</p>
<p>Initial Service Term: 1 month</p>
<p>This Services Order Form is a binding, legal agreement between the Company that accesses and uses the Services and Skedular Ltd (trading as Skedular) (the “Company”, “us”, or “our”) for the provision of the Services and is subject to the Terms of Service set out below.</p>
<p>By accessing and using the Services, you represent that (1) you have read, understand, and agree to be bound by the terms of use, (2) you have the authority to enter into the terms of use personally or on behalf of the Company, and to bind that Company to the terms of use.</p>
<p>If you subscribe to the services for a term (the “Initial Term”), then the terms will be automatically renewed for additional periods of the same duration as the Initial Term at the Company’s then-current fee for such services unless you opt out of the auto-renewal.</p>
<h3>TERMS OF SERVICE</h3>
<h4>1. SAAS SERVICES AND SUPPORT</h4>
<p>1.1. Subject to the Services Order Form above and these Terms of Service (including any Exhibits to it) as amended from time to time), The company will use commercially reasonable efforts to provide the Services to the Customer during the Term (as defined in Section 5.1 below).</p>
<p>1.2. As part of the registration process, the Customer will identify an administrative username and password for the Customer’s Company account. The company reserves the right to refuse registration of or cancel passwords it deems inappropriate.</p>
<p>1.3. Customer recognizes that the Company is always innovating and finding ways to improve the Services with new features and services. Customer therefore agrees that the Services may change from time to time and no warranty, representation, or other commitment is given in relation to the continuity of any functionality of the Service.</p>
<p>1.4. Subject to the terms hereof, the Company will provide Customer with reasonable technical support services in accordance with the terms set forth in Exhibit A.</p>
<h4>2. RESTRICTIONS AND RESPONSIBILITIES</h4>
<p>2.1. Company hereby grants to Customer a non-exclusive, non-transferable, non-sublicensable right to access and use the Services during the Term in accordance with the Agreement, solely for Customer’s internal business purposes.</p>
<p>2.2 Customer will not, directly or indirectly (except to the extent permitted by any applicable law which is incapable of exclusion by the agreement of the parties): reverse engineer, decompile, disassemble or otherwise attempt to discover the source code, object code or underlying structure, ideas, know-how or algorithms relevant to the Services or any software, documentation or data related to the Services (“Software”); modify, translate, or create derivative works based on the Services or any Software(except to the extent expressly permitted by Company or authorized within the Services); license, sell, rent, lease, transfer, assign, distribute, display, disclose or otherwise commercially exploit the Services and any Software (except to the extent expressly permitted by Company or authorized within the Services); use the Services or any Software for timesharing or service bureau purposes or otherwise for the benefit of a third party; or remove any proprietary notices or labels. With respect to any Software that is distributed or provided to Customer for use on Customer premises or devices, the Company hereby grants Customer a non-exclusive, non-transferable, non-sublicensable license to use such Software during the Term only in connection with the Services.</p>
<p>2.3. Customer shall comply with all applicable technology control and export laws and regulations.</p>
<p>2.4. Customer represents, covenants, and warrants that Customer will use the Services only in compliance with the terms and conditions of the Agreement, and all applicable laws and regulations. Customer hereby agrees to indemnify and hold harmless Company against any damages, losses, liabilities, settlements, and expenses (including without limitation costs and attorneys’ fees) in connection with any claim or action that arises from any actual or alleged breach of the terms and conditions of the Agreement, and/or any applicable laws and regulations, or otherwise from Customer’s use of Services. Although the Company has no obligation to monitor the Customer’s use of the Services, the Company may do so and may prohibit any use of the Services it believes may be (or alleged to be) in violation of the Agreement.</p>
<p>2.5. Customer shall be responsible for obtaining and maintaining any equipment and ancillary services needed to connect to, access, or otherwise use the Services, including, without limitation, modems, hardware, servers, software, operating systems, networking, web servers, and the like (collectively, “Equipment”). Customer shall also be responsible for maintaining the security of the Equipment, Customer account, passwords (including but not limited to administrative and user passwords), and files, and for all uses of Customer account or the Equipment with or without Customer’s knowledge or consent.</p>
<h4>3. CONFIDENTIALITY; PROPRIETARY RIGHTS</h4>
<p>3.1. Each party undertakes that it shall not at any time during this Agreement, and for a period of two years after termination or expiry of this Agreement, disclose to any person any confidential information concerning the business, affairs, customer, clients or suppliers or the other party or of any member of the group of companies to which the other party belongs, except as permitted by clause 3.2 below.</p>
<p>3.2 Notwithstanding clause 3.1, each party may disclose the other party’s confidential information:</p>
<ul>
<li>to its employees, officers, representatives, contractors, subcontractors, or advisers who need to know such information for the purposes of exercising the party’s rights or carrying out its obligations under or in connection with this Agreement. Each party shall ensure that its employees, officers, representatives, contractors, subcontractors, or advisers to whom it discloses the other party’s confidential information comply with this clause 3; and</li>
<li>as may be required by law, a court of competent jurisdiction, or any governmental or regulatory authority.</li>
</ul>
<p>3.3 No party shall use any other party’s confidential information for any purpose other than to exercise its rights and perform its obligations under or in connection with this Agreement.</p>
<p>3.4. Customer shall own and retain all rights, titles, and interest in and to the Customer Data, and shall have sole responsibility for the legality, reliability, integrity, accuracy, and quality of the Customer Data.</p>
<p>3.5. The Company shall own and retain all rights, titles, and interests in and to (a) the Services and Software, all improvements, enhancements, or modifications thereto,(b) any software, applications, inventions, or other technology developed in connection with Services or support, and (c) all intellectual property rights related to any of the foregoing.</p>
<p>3.6. Notwithstanding anything to the contrary, the Company shall have the right to collect and analyze data and other information relating to the provision, use, and performance of various aspects of the Services and related systems and technologies (including, without limitation, information concerning Customer Data and data derived therefrom), and Company will be free (during and after the Term) to (i) use such information for the purpose of providing the Services; (ii) use such information and data to improve and enhance the Services and for other development, diagnostic and corrective purposes in connection with the Services and other Company offerings, and (iii) disclose such data solely in aggregate or other de-identified forms in connection with its business. No rights or licenses are granted except as expressly set forth herein.</p>
<h4>4. PAYMENT OF FEES</h4>
<p>4.1. Customer will pay Company the then applicable Service Fees. Company reserves the right to change the Service Fees or applicable charges and to institute new charges and Service Fees at the end of the Initial Service Term or then-current renewal term, upon thirty (30) days prior notice to Customer (which may be sent by email). If Customer believes that Company has billed Customer incorrectly, Customer must contact Company no later than 60 days after the closing date on the first billing statement in which the error or problem appeared, in order to receive an adjustment or credit. Inquiries should be directed to the Company’s customer support department.</p>
<p>4.2. Company may choose to bill through an invoice, in which case, full payment for invoices issued in any given month must be received by the Company ten (10) days after the mailing date of the invoice. Unpaid amounts are subject to interest at an annual rate of 4% over the then-current base lending rate of Australia and New Zealand Bank (ANZ Bank) from time to time on any outstanding balance, or the maximum permitted by law, whichever is lower, commencing on the due date for payment and continuing until payment is received in full by Company, plus all expenses of collection, and may result in immediate termination of the Service. Customer shall be responsible for all taxes associated with Services other than applicable taxes based on the Company’s net income.</p>
<p>4.3. For the purpose of calculating the Service Fees, the term “Additional Revenue” means any revenue generated by the Customer arising from the Customer’s use of the Services (prior to the addition of taxes).</p>
<h4>5. TERM AND TERMINATION</h4>
<p>5.1. Subject to earlier termination as provided below, the Agreement is for the Initial Service Term and shall be automatically renewed for additional periods of the same duration as the Initial Service Term (collectively, the “Term”), unless either party gives the other party at least ten (10) days’ notice in writing to terminate the Agreement at the end of the then-current term. A reason for this termination does not need to be given.</p>
<p>5.2. Without prejudice to any other rights or remedies available to it, the Company may, without liability, immediately disable Customer’s account for the Services or prevent access by Customer to the Services for any breach by the Customer of this Agreement.</p>
<p>5.3. In addition to any other remedies it may have, either party may also terminate the Agreement immediately on written notice (or without notice in the case of non-payment), if the other party (i) materially breaches any of the terms or conditions of the Agreement which breach is irremediable or (if such breach is remediable) fails to remedy the breach within ten (10) days of being notified in writing to do so, or (ii) is unable to pay its debts (within the meaning of section 123 of the Insolvency Act 1986) or becomes insolvent, or is subject to an order or a resolution for its liquidation, administration, winding-up or dissolution (otherwise than for the purposes of a solvent amalgamation or reconstruction), or has an administrative or other receiver, manager, trustee, liquidator, administrator or similar officer appointed over all or any substantial part of its assets, or enters into or proposes any composition or arrangement with its creditors generally, or is subject to any analogous event or proceeding in any applicable jurisdiction. The customer will pay in full for the Services up to and including the last day on which the Services are provided. Upon any termination (i) all licenses granted under the Agreement shall immediately terminate and Customer’s right to access and use the Services will end, and (ii) (subject to the terms and conditions of the DPA) Company will make all Customer Data available to Customer for electronic retrieval for a period of thirty (30) days, but thereafter, Company may, but is not obligated to, delete stored Customer Data. All sections of the Agreement which by their nature should survive termination will survive termination, including, without limitation, accrued rights to payment, confidentiality obligations, warranty disclaimers, and limitations of liability.</p>
<h4>6. WARRANTY AND DISCLAIMER</h4>
<p>6.1 The Company shall use reasonable efforts consistent with prevailing industry standards to maintain the Services in a manner that minimizes errors and interruptions in the Services and shall perform the Services in a professional and workmanlike manner. Services may be temporarily unavailable for scheduled maintenance or for unscheduled emergency maintenance, either by Company or by third-party providers, or because of other causes beyond Company’s reasonable control, but Company shall use reasonable efforts to provide advance notice in writing or by e-mail of any scheduled service disruption.</p>
<p>6.2 The Company does not warrant that the Services will be uninterrupted or error-free; nor does it make any warranty as to the results that may be obtained from the use of the Services. Except as expressly set forth in this section, the services are provided “AS IS” and the Company disclaims and excludes from the Agreement to the fullest extent permitted by applicable law all warranties, representations, conditions, and all other terms of any kind whatsoever, express or implied by statute or common law or otherwise, including, but not limited to, implied warranties of merchantability and fitness for a particular purpose and non-infringement.</p>
<h4>7. LIMITATION OF LIABILITY</h4>
<p>7.1 Nothing in the Agreement excludes the liability of either party for death or personal injury caused by its negligence, or for fraud or fraudulent misrepresentation.</p>
<p>7.2 Subject to clause 7.1, Company and its suppliers (including but not limited to all equipment and technology suppliers), officers, affiliates, representatives, contractors and employees shall not be responsible or liable with respect to any subject matter of the Agreement or terms and conditions related thereto under any contract, tort (including negligence), for breach of statutory duty, or otherwise: (a) for error or interruption of use of the Services or for loss, alteration, or inaccuracy or corruption of data (including Customer Data) or cost of procurement of substitute goods,services or technology; (b) for any loss (whether direct or indirect) of profits, revenue, business, or goodwill; (c) for any indirect, exemplary, incidental, special or consequential loss, costs, damages, charges or expenses; (d) for any matter beyond Company’s reasonable control; or (e) for any amounts that, together with amounts associated with all other claims, exceed the fees paid by Customer to Company for the Services under the Agreement in the 12 months prior to the act that gave rise to the liability, in each case, whether or not Company has been advised of the possibility of such damages.</p>
<h4>8. MISCELLANEOUS</h4>
<p>8.1 Company may use Customer’s name, logo, and related trademarks in any of Company’s publicity or marketing materials for the purpose of highlighting that Customer uses the Services, and alongside any testimonials that Customer has agreed to give.</p>
<p>8.2 Company shall have no liability to Customer under the Agreement if it is prevented from or delayed in performing its obligations under the Agreement by any act, event or omission beyond its control, including (without limitation): strikes, lock-outs, or other industrial disputes; failure of a utility service or transport or telecommunications network; act of God, fire, flood, or storm; war, riot, or civil commotion; malicious damage; compliance with any law or governmental order, rule, regulation or direction; accident; breakdown of machinery; or default of suppliers or sub-contractors. Company shall notify Customer of any such event and (where possible) its expected duration.</p>
<p>8.3 If any provision of the Agreement is found to be unenforceable or invalid, that provision will be limited or eliminated to the minimum extent necessary so that the Agreement will otherwise remain in full force and effect and enforceable.</p>
<p>8.4 If there is an inconsistency between any of the provisions in the main body of the Agreement and the Exhibits, the provisions in the Exhibits shall prevail to the extent of the inconsistency.</p>
<p>8.5 The Agreement is not assignable, transferable or sublicensable by Customer except with Company’s prior written consent. Company may transfer and assign any of its rights and obligations under the Agreement without the consent of Customer.</p>
<p>8.6 The Agreement(including the Exhibits and any documents referred to in it) constitutes the entire agreement between the parties and is the complete and exclusive statement of the mutual understanding of the parties and supersedes and cancels all previous written and oral agreements, communications, and other understandings relating to the subject matter of the Agreement. All waivers and modifications must be in writing and signed by both parties, except as otherwise provided herein.</p>
<p>8.7 No failure or delay by either party to exercise any right or remedy provided under the Agreement shall constitute a waiver of that or any other right or remedy, nor shall it prevent or restrict the further exercise of that or any other right or remedy.</p>
<p>8.8 No agency, partnership, joint venture, or employment is created as a result of the Agreement and Customer does not have any authority of any kind to bind Company in any respect whatsoever.</p>
<p>8.9 The Agreement does not confer any rights on any person or party (other than the parties to the Agreement), under the Contracts (Rights of Third Parties) Act 1999 or otherwise.</p>
<p>8.10 Applicable laws may require that some of the information or communications the Company sends to the Customer should be in writing. When using the Services, Customer accepts that communication with Company will mainly be electronic and Company may provide information to Customer by posting notices on the Services. All notices under the Agreement will be in writing and will be deemed to have been duly given when received if personally delivered; when receipt is electronically confirmed if transmitted by facsimile or e-mail; the day after it is sent if sent for the next day delivery by recognized overnight delivery service; and upon receipt, if sent by certified or registered mail, return receipt requested, provided that Company may give notice to Customer at either the e-mail or postal address Customer provides to Company or any other way Company deems appropriate. The foregoing notice provisions do not apply to the termination of the Agreement, or to the service of any proceedings or other documents in any legal action or dispute resolution.</p>
<p>8.11 The Agreement and any dispute or claim arising out of or in connection with it or its subject matter or formation (including non-contractual disputes or claims) shall be governed by and construed in accordance with the law of New Zealand.</p>
<p>8.12 Each party irrevocably agrees that the courts of New Zealand shall have exclusive jurisdiction to settle any dispute or claim arising out of or in connection with the Agreement or its subject matter or formation (including non-contractual disputes or claims).</p>
<p>8.13 Company has the right to revise and amend these Terms of Service from time to time. Changes to these Terms of Service are effective when they are posted on this page, and Customer will be subject to the Terms of Service in force at the time that it makes use of the Services (unless otherwise agreed by the parties in writing).</p>
<h3>EXHIBIT A</h3>
<h4>Support Terms</h4>
<p>The company will use commercially reasonable efforts to respond to all Helpdesk tickets within one(1) business day</p>
<p>The company will provide Technical Support to the Customer via both telephone and electronic mail on weekdays during the hours of 9:00 am New Zealand time through to 5:00 pm, with the exclusion of Public Holidays in New Zealand (“Support Hours”).</p>
<h3>EXHIBIT B</h3>
<h3>Data Processing Addendum</h3>
<h3>SKEDULAR DATA PROCESSING ADDENDUM</h3>
<h4>1. BACKGROUND</h4>
<p>1.1. The Customer and Skedular Limited (“Skedular”, “the Company”, “we”, “us”, or “our”) entered into the Agreement (as defined below) for the provision of the Services (as defined in the Agreement) from Skedular to the Customer.</p>
<p>1.2. In the event that Skedular Processes personal data (each as defined below) contained in Customer Data of individuals located in New Zealand (as defined below), or if the Customer is established in New Zealand, this Data Processing Addendum (the “DPA”) shall be supplemental to the Agreement and shall apply to the Processing of such personal data. In the event of a conflict between any of the provisions of this DPA and the provisions of the Agreement, the provisions of this DPA shall prevail.</p>
<p>1.3. Both parties will comply with all applicable requirements of the Data Protection Laws (as defined below). This DPA is in addition to and does not relieve, remove, or replace, a party’s obligations under the Data Protection Laws.</p>
<h4>2. DEFINITIONS</h4>
<p>2.1. Unless otherwise set out below, each capitalized term in this DPA shall have the meaning set out in the Agreement, and the following capitalized terms used in this DPA shall be defined as follows:</p>
<p>2.1.1. “Agreement” means the agreement entered into between the Company and the Customer for the provision of the Service, comprising the Order Form and the Terms of Service (including the Exhibits to it);</p>
<p>2.1.2. “Controller” has the meaning given in the Data Protection Laws;</p>
<p>2.1.3. “Customer Personal Data” means the personal data (as defined in the Data Protection Laws) described in ANNEX 1 and any other personal data that Skedular processes on behalf of the Customer in connection with Skedular’s provision of the Service;</p>
<p>2.1.4. “Data Protection Laws” means: Coming soon</p>
<p>2.1.5. “Data Subject” has the meaning given in the Data Protection Laws;</p>
<p>2.1.6. “EU GDPR” means the General Data Protection Regulation ((EU) 2016/679);</p>
<p>2.1.7. “European Economic Area” or “EEA” means the Member States of the European Union together with Iceland, Norway, and Liechtenstein;</p>
<p>2.1.8. “Processing” has the meaning given in the Data Protection Laws, and “Process” shall be interpreted accordingly; Data Protection Laws GDPR;</p>
<p>2.1.9. “Security Incident” means any accidental or unlawful destruction, loss, alteration, unauthorized disclosure of, or access to, any Customer Personal Data;</p>
<p>2.1.10. “Standard Contractual Clauses” means the Standard Contractual Clauses (processors) approved by the European Commission Decision on 4 June 2021 or any subsequent version thereof released by the European Commission (which will automatically apply);</p>
<p>2.1.11. “Subprocessor” means any Processor engaged by Skedular that agrees to receive from Skedular and Process any Customer Personal Data; and</p>
<p>2.1.12. “Supervisory has the meaning given to it in section 3(10) (as supplemented by section 205(4)) of the Data Protection Act 2018.</p>
<h4>3. DATA PROCESSING</h4>
<p>3.1. Instructions for Data Processing. Skedular will only Process Customer Personal Data in accordance with the Customer’s instructions unless Processing is required by Data Protection Laws to which Skedular is subject, in which case Skedular shall, to the extent permitted by Data Protection Laws, inform the Customer of that legal requirement before Processing that Customer Personal Data. The Agreement (subject to any changes to the Services agreed between the Parties), including this DPA, shall be the Customer’s complete and final instructions to Skedular in relation to the processing of Customer Personal Data.</p>
<p>3.2. Processing outside the scope of this DPA or the Agreement will require a prior written agreement between the Customer and Skedular on additional instructions for Processing.</p>
<p>3.3. Required consents. Where required by applicable Data Protection Laws, the Customer warrants that it will ensure that it has obtained/will obtain all necessary consents for the Processing of Customer Personal Data by Skedular in accordance with the Agreement, and agrees to indemnify Skedular for any direct losses arising out of a breach of this clause.</p>
<h4>4. TRANSFER OF PERSONAL DATA</h4>
<p>4.1. Authorised Subprocessors. The Customer agrees that Skedular may use Amazon Web Services, Inc., Stripe Inc., Slack Technologies LLC, LogRocket, Inc., and Microanalytics.io as Subprocessors to Process Customer Personal Data, together with additional subcontractors when required from time to time, which the Customer hereby approves in advance.</p>
<p>4.2. Save as set out in clauses 4.1, Skedular shall not permit, allow, or otherwise facilitate Subprocessors to Process Customer Personal Data unless Skedular enters into a written agreement with the Subprocessor which imposes the same obligations on the Subprocessor with regards to their Processing of Customer Personal Data as are imposed on Skedular under this DPA.</p>
<p>4.3. Liability of Subprocessors. Skedular shall at all times remain responsible for compliance with its obligations under the DPA and will be liable to the Customer for the acts and omissions of any Subprocessors if they were the acts and omissions of Skedular.</p>
<p>4.4. Transfers of Personal Data. To the extent that the Processing of Customer Personal Data by Skedular involves the export of such Customer Personal Data to a third party to a country or territory outside New Zealand, other than (i) a country or territory ensuring inadequate level of protection for the rights and freedoms of Data Subjects in relation to the Processing of personal data as determined by the European Commission, or (ii) where the third party is a member of a compliance scheme recognized as offering adequate protection for the rights and freedoms of Data Subjects as determined by the European Commission, such export shall be governed by the Standard Contractual Clauses between the Customer as exporter and such third party as importer. For this purpose, the Customer appoints Skedular as its agent with the authority to complete and enter into the Standard Contractual Clauses as an agent for the Customer on its behalf for this purpose.</p>
<p>4.5. In the event of any conflict between any terms and conditions of the Standard Contractual Clauses and this DPA, the Standard Contractual Clauses shall prevail.</p>
<h4>5. DATA SECURITY, AUDITS AND SECURITY NOTIFICATIONS</h4>
<p>5.1. Skedular Security Obligations. Taking into account state of the art, the costs of implementation, and the nature, scope, context, and purposes of Processing, as well as the risk of varying likelihood and severity for the rights and freedoms of natural persons, Skedular shall implement appropriate technical and organizational measures to ensure a level of security appropriate to the risk, including the measures set out in ANNEX 2.</p>
<p>5.2. Security Audits. The Customer may, upon reasonable notice, audit(by itself or using independent third-party auditors) Skedular’s compliance with the security measures set out in this DPA (including the technical and organizational measures as set out in ANNEX 2) no more than once per year, including by conducting audits of Skedular’s data processing facilities. Upon request by the Customer, Skedular shall make available all information reasonably necessary to demonstrate compliance with this DPA.</p>
<p>5.3. Security Incident Notification. If Skedular or any Subprocessor becomes aware of a Security Incident, Skedular will (a) notify the Customer of the Security Incident within 72 hours of becoming aware of the Security Incident, (b) investigate the Security Incident and provide such reasonable assistance to the Customer (and any law enforcement or regulatory official) as required to investigate the Security Incident, and (c) take steps to remedy any non-compliance with this DPA.</p>
<p>5.4. Skedular Employees and Personnel. Skedular shall treat the Customer Personal Data as the Confidential Information of the Customer and shall ensure that any employees or other personnel have agreed in writing to protect the confidentiality and security of Customer Personal Data.</p>
<h4>6. ACCESS REQUESTS AND DATA SUBJECT RIGHTS</h4>
<p>6.1. Data Subject Requests. Save as required (or where prohibited) under applicable law, Skedular shall notify the Customer of any request received by Skedular or any Subprocessor from a Data Subject in respect of their personal data included in the Customer Personal Data, and shall not respond to the Data Subject.</p>
<p>6.2. Skedular shall provide the Customer with the ability to correct, delete, block, access, or copy the Customer’s Personal Data in accordance with the functionality of the Service.</p>
<p>6.3. Government Disclosure. Skedular shall notify the Customer of any request for the disclosure of Customer Personal Data by a governmental or regulatory body or law enforcement authority (including any data protection supervisory authority) unless otherwise prohibited by law or a legally binding order of such body or agency.</p>
<p>6.4. Data Subject Rights. Where applicable, and taking into account the nature of the Processing, Skedular shall use all reasonable endeavors to assist the Customer by implementing any other appropriate technical and organizational measures, insofar as this is possible, for the fulfillment of the Customer’s obligation to respond to requests for exercising Data Subject rights laid down in the GDPR.</p>
<p>6.5. Data Protection Impact Assessment and Prior Consultation. To the extent required under applicable Data Protection Laws, Skedular shall provide reasonable assistance to the Customer with any data protection impact assessments and with any prior consultations to any Supervisory Authority of the Customer, in each case solely in relation to the Processing of Customer Personal Data and taking into account the nature of the Processing and information available to Skedular.</p>
<h4>7. TERMINATION</h4>
<p>7.1. Subject to clause 7.2 below, the Customer may in its absolute discretion notify Skedular in writing within thirty (30) days of the date of termination of the Agreement to require Skedular to delete and procure the deletion of all copies of Customer Personal Data Processed by Skedular. Skedular shall, within ninety (90) days of the date of termination of the Agreement:</p>
<p>7.1.1.comply with any such written request; and</p>
<p>7.1.2.use all reasonable endeavors to procure that its Subprocessors delete all Customer Personal Data Processed by such Subprocessors,</p>
<p>7.1.3.and, where this clause 7.2 applies, Skedular shall not be required to provide a copy of the Customer’s Personal Data to the Customer.</p>
<p>7.2. Skedularand its Subprocessors may retain Customer Personal Data to the extent required by applicable laws and only to the extent and for such period as required by applicable laws and always provided that Skedular shall ensure the confidentiality of all such Customer Personal Data and shall ensure that such Customer Personal Data is only Processed as necessary for the purpose(s) specified in the applicable laws requiring its storage and for no other purpose.</p>
<h3>ANNEX 1</h3>
<h4>Details of the Processing of CUSTOMER Personal Data</h4>
<p>This ANNEX 1 includes certain details of the processing of Customer Personal Data.</p>
<p>Subject matter and duration of the Processing of Customer Personal Data</p>
<p>The subject matter and the duration of the Processing of the Customer’s Personal Data are set out in the Agreement and this DPA.</p>
<p>The nature and purpose of the Processing of Customer Personal Data</p>
<p>The Customer’s Personal Data will be subject to the following basic Processing activities: transmitting, collecting, storing, and analyzing data in order to provide the Services to the Customer, and any other activities related to the provision of the Services or as specified in the Agreement.</p>
<p>The types of Customer Personal Data to be Processed</p>
<p>The types of Customer Personal Data to be Processed concern the following categories of data: names of Customer personnel; contact information (including email addresses and telephone numbers) of Customer personnel and of end users of services of the Customer; online identifiers of end users of services of the Customer.</p>
<p>The categories of Data Subject to whom the Customer’s Personal Data relates</p>
<p>The categories of Data Subject to whom the Customer’s Personal Data relates concern: employees and other personnel of the Customer.</p>
<p>The obligations and rights of the Customer</p>
<p>The obligations and rights of the Customer are as set out in the Agreement and this DPA.</p>
<h3>ANNEX 2</h3>
<h4>Technical and Organisational Security Measures</h4>
<p>Skedular maintains internal policies and procedures, or procures that its Subprocessors do so, which are designed to:</p>
<ul>
<li>secure any Customer Personal Data Processed by Skedular against accidental or unlawful loss, access, or disclosure;</li>
<li>identify reasonably foreseeable and internal risks to security and unauthorized access to the Customer’s Personal Data Processed by Skedular;</li>
<li>minimize security risks, including risk assessment and regular testing.</li>
</ul>
<p>Skedular will, and will use reasonable efforts to procure that its Subprocessors will, conduct periodic reviews of the security of its network and the adequacy of its information security program as measured against industry security standards and its policies and procedures.</p>
<p><strong>Skedular will, and will use reasonable efforts to procure that its Subprocessors periodically will, evaluate the security of its network and associated services to determine whether additional or different security measures are required to respond to new security risks or findings generated by the periodic reviews.</strong></p>`,
  },
  {
    id: "privacy-policy",
    path: "/privacy-policy",
    title: "Privacy Policy | Skedular",
    description:
      "Review Skedular's Privacy Policy for how personal data is collected, used, disclosed, secured, retained, and protected.",
    sourceUrl: "https://getskedular.com/privacy-policy/",
    reviewStatus: "pending",
    lastSourceReview: "2026-06-05",
    heroSummary:
      "This Privacy Policy is migrated from the current Skedular legal source and preserves the source legal document wording.",
    notice:
      "Legal document migrated from the current Skedular Privacy Policy source. Do not rewrite or summarize this content without business or legal approval.",
    html: String.raw`<p><span>Welcome to our privacy policy. We respect your privacy and are committed to protecting your personal data. This privacy policy lets you know how we look after your personal data when you visit our website (regardless of where you visit it from) or access our services, and tells you about your privacy rights and how the law protects you.</span></p>
<h3>Important information and who we are</h3>
<h4>Purpose of this privacy policy</h4>
<p>This privacy policy aims to give you information on how Skedular collects and processes your personal data through your use of this website and/or our Slack integration, including any data you may provide when accessing or using our services.</p>
<p>Our website and services are not intended for children and we do not knowingly collect data relating to children.</p>
<p>It is important that you read this privacy policy together with any other privacy policy or fair processing policy we may provide on specific occasions when we are collecting or processing personal data about you so that you are fully aware of how and why we are using your data. This privacy policy supplements other notices and privacy policies and is not intended to override them</p>
<h4>Controller</h4>
<p>Skedular Limited, trading as Skedular, is the data controller and is responsible for your personal data(collectively referred to as “Skedular”, “we”, “us” or “our” in this privacy policy).</p>
<p>If you have any questions about this privacy policy, including any requests to exercise your legal rights, please contact us using the details set out below.</p>
<p>Full name of legal entity: Skedular Limited, trading as Skedular.</p>
<p>Email address: privacypolicy@getskedular.com</p>
<p>Postal address:</p>
<p>You have the right to make a complaint at any time to your local data protection authority. We would, however, appreciate the chance to deal with your concerns before you approach the data protection authority so please contact us in the first instance.</p>
<h4>Changes to the privacy policy and your duty to inform us of changes</h4>
<p>We keep our privacy policy under regular review.</p>
<p>It is important that the personal data we hold about you is accurate and current. Please keep us informed if your personal data changes during your relationship with us.</p>
<h4>Third-party links</h4>
<p>This website may include links to third-party websites, plug-ins, and applications. Clicking on those links or enabling those connections may allow third parties to collect or share data about you. We do not control these third-party websites and are not responsible for their privacy statements. When you leave our website, we encourage you to read the privacy policy of every website you visit.</p>
<h3>The data we collect about you</h3>
<p>Personal data, or personal information, means any information about an individual from which that person can be identified. It does not include data where the identity has been removed (anonymous data).</p>
<p>We may collect, use, store and transfer different kinds of personal data about you which we have grouped together as follows:</p>
<ul>
<li>Identity and Contact Data includes name, username (including Slack username) or similar identifier and email address.</li>
<li>Technical Data includes internet protocol (IP) address, your login data, browser type and version, time zone setting and location, browser plug-in types and versions, operating system and platform, and other technology on the devices you use to access this website or our services.</li>
<li>Usage Data includes information about how you use our website and services.</li>
<li>Marketing and Communications Data includes your preferences in receiving marketing from us and your communication preferences.</li>
</ul>
<p>We also collect, use and share Aggregated Data such as statistical or demographic data for any purpose. Aggregated Data could be derived from your personal data but is not considered personal data in law as this data will not directly or indirectly reveal your identity. For example, we may aggregate your Usage Data to calculate the percentage of users accessing a specific website feature. However, if we combine or connect Aggregated Data with your personal data so that it can directly or indirectly identify you, we treat the combined data as personal data which will be used in accordance with this privacy policy.</p>
<p>We do not collect any Special Categories of Personal Data about you (this includes details about your race or ethnicity, religious or philosophical beliefs, sex life, sexual orientation, political opinions, trade union membership, information about your health, and genetic and biometric data). Nor do we collect any information about criminal convictions and offences.</p>
<h4>If you fail to provide personal data</h4>
<p>Where we need to collect personal data by law, or under the terms of a contract we have with you, and you fail to provide that data when requested, we may not be able to perform the contract we have or are trying to enter into with you (for example, to provide you with goods or services). In this case, we may have to cancel a product or service you have with us but we will notify you if this is the case at the time.</p>
<h3>How is your personal data collected?</h3>
<p>We use different methods to collect data from and about you including through:</p>
<ul>
<li>Direct interactions. You may give us, or we may automatically receive, your Identity and Contact Data when you use our services.</li>
<li>Automated technologies or interactions. As you interact with our website and services, we will automatically collect Technical Data about your equipment, browsing actions and patterns. We collect this personal data by using cookies and other similar technologies.</li>
<li>Third parties or publicly available sources. We may automatically receive your Identity and Contact Data when the company you work for integrates their Slack application with our services.</li>
</ul>
<h3>How we use your personal data</h3>
<p>We will only use your personal data when the law allows us to. Most commonly, we will use your personal data in the following circumstances:</p>
<ul>
<li>Where we need to perform a contract we are about to enter into or have entered into with you.</li>
<li>Where it is necessary for our legitimate interests (or those of a third party) and your interests and fundamental rights do not override those interests.</li>
<li>Where we need to comply with a legal obligation.</li>
</ul>
<p>Generally, we do not rely on consent as a legal basis for processing your personal data although, when required by law, we will get your consent before sending direct marketing communications to you via email or text message. You have the right to withdraw consent to marketing at any time by contacting us.</p>
<h4>Promotional offers from us</h4>
<p>We may use your Identity, Contact, Technical, Usage and Profile Data to form a view on what we think you may want or need, or what may be of interest to you. This is how we decide which products, services and offers may be relevant for you.</p>
<h4>Opting out</h4>
<p>You can ask us or third parties to stop sending you marketing messages by contacting us at any time.</p>
<h4>Change of purpose</h4>
<p>We will only use your personal data for the purposes for which we collected it, unless we reasonably consider that we need to use it for another reason and that reason is compatible with the original purpose. If you wish to get an explanation as to how the processing for the new purpose is compatible with the original purpose, please contact us.</p>
<p>If we need to use your personal data for an unrelated purpose, we will notify you and we will explain the legal basis which allows us to do so.</p>
<p>Please note that we may process your personal data without your knowledge or consent, in compliance with the above rules, where this is required or permitted by law.</p>
<h3>Disclosures of your personal data</h3>
<p>We may share your personal data with the parties set out below for the purposes set out in the table above.</p>
<ul>
<li>External service providers that we work with operate our business, website and services.</li>
<li>Third parties to whom we may choose to sell, transfer or merge parts of our business or our assets. Alternatively, we may seek to acquire other businesses or merge with them. If a change happens to our business, then the new owners may use your personal data in the same way as set out in this privacy policy.</li>
</ul>
<p>We require all third parties to respect the security of your personal data and to treat it in accordance with the law. We do not allow our third-party service providers to use your personal data for their own purposes and only permit them to process your personal data for specified purposes and in accordance with our instructions.</p>
<h4>International transfers</h4>
<p>Some of the external service providers to which we need to transfer personal data are based outside the New Zealand. This means that their processing of your personal data will involve a transfer of data outside the New Zealand.</p>
<p>Whenever we transfer your personal data out of the New Zealand, we ensure a similar degree of protection is afforded to it by ensuring that appropriate safeguards are implemented.</p>
<h3>Data security</h3>
<p>We have put in place appropriate security measures to prevent your personal data from being accidentally lost, used or accessed in an un authorised way, altered or disclosed. In addition, we limit access to your personal data to those employees, agents, contractors and other third parties who have a business need to know. They will only process your personal data on our instructions and they are subject to a duty of confidentiality.</p>
<p>We have put in place procedures to deal with any suspected personal data breach and will notify you and any applicable regulator of a breach where we are legally required to do so.</p>
<h3>Data retention</h3>
<h4>How long will you use my personal data for?</h4>
<p>We will only retain your personal data for as long as reasonably necessary to fulfil the purposes we collected it for, including for the purposes of satisfying any legal, regulatory, tax, accounting or reporting requirements. We may retain your personal data for a longer period in the event of a complaint or if we reasonably believe there isa prospect of litigation in respect to our relationship with you.</p>
<p>To determine the appropriate retention period for personal data, we consider the amount, nature and sensitivity of the personal data, the potential risk of harm from unauthorised use or disclosure of your personal data, the purposes for which we process your personal data and whether we can achieve those purposes through other means,and the applicable legal, regulatory, tax, accounting or other requirements.</p>
<h3>Your legal rights</h3>
<p>Under certain circumstances, you have various rights under data protection laws in relation to your personal data, including the right to:</p>
<ul>
<li>request access to your personal data;</li>
<li>request correction of your personal data;</li>
<li>request erasure of your personal data;</li>
<li>object to processing of your personal data;</li>
<li>request restriction of processing your personal data;</li>
<li>request transfer of your personal data; and</li>
<li>withdraw consent.</li>
</ul>
<p>If you wish to exercise any of the rights set out above, please contact us.</p>
<h4>No fee usually required</h4>
<p>You will not have to pay a fee to access your personal data (or to exercise any of the other rights). However, we may charge a reasonable fee if your request is clearly unfounded, repetitive or excessive. Alternatively, we could refuse to comply with your request in these circumstances.</p>
<h4>What we may need from you</h4>
<p>We may need to request specific information from you to help us confirm your identity and ensure your right to access your personal data (or to exercise any of your other rights). This is a security measure to ensure that personal data is not disclosed to any person who has no right to receive it. We may also contact you to ask you for further information in relation to your request to speed up our response.</p>
<h4>Time limit to respond</h4>
<p>We try to respond to all legitimate requests within one month. Occasionally it could take us longer than a month if your request is particularly complex or you have made a number of requests. In this case, we will notify you and keep you updated.</p>`,
  },
];

export function findLegalPage(id: LegalPage["id"]): LegalPage {
  const page = legalPages.find((item) => item.id === id);

  if (!page) {
    throw new Error(`Legal page not found: ${id}`);
  }

  return page;
}
