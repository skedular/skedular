/** @type {import('@eventcatalog/core/bin/eventcatalog.config').Config} */
export default {
  cId: 'de1ba2e6-3f5b-487f-ba03-d70f5c329e8a',
  title: "Skedular",
  tagline: "Skeudlar Documentation",
  organizationName: "Skedular",
  homepageLink: "https://getskedular.com/",
  // By default set to false, add true to get urls ending in /
  trailingSlash: false,
  // Change to make the base url of the site different, by default https://{website}.com/docs,
  // changing to /company would be https://{website}.com/company/docs,
  base: "/",
  // Customize the logo, add your logo to public/ folder
  logo: {
    alt: "Skedular Logo",
    src: "/skedular-logo-primary.svg",
    text: "Skedular",
  },
  docs: {
    sidebar: {
      // Should the sub heading be rendered in the docs sidebar?
      showPageHeadings: true,
    },
  },
};
