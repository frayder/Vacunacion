/**
 * Theme: Highdmin - Responsive Bootstrap 5 Admin Dashboard
 * Author: I Analitycs
 * Module/App: Inbox 
 */


const quill = new Quill('#mail-compose', {
    modules: {
        toolbar: [
            [{ header: [1, 2, false] }],
            ['bold', 'italic', 'underline'],
            ['image', 'code-block'],
        ],
    },
    placeholder: 'Compose an epic...',
    theme: 'snow', // or 'bubble'
});