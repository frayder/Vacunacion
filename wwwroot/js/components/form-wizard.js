/**
* Theme: Highdmin - Responsive Bootstrap 5 Admin Dashboard
* Author: I Analitycs
* Module/App: Form Wizard
*/



new Wizard('#basicwizard');

new Wizard('#progressbarwizard', {
  progress: true
});

new Wizard('#validation-wizard', {
  validate: true
});