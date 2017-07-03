﻿using ContactApp.DataHandlers;
using ContactApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace ContactApp.Pages
{
    public partial class ContactList : ContentPage
    {
        // T. : est-ce vraiment nécessaire de devoir gérer des variables avec un swipe normal ?
        private float SwipeDistance = 0;
        private float ReferenceSwipeDistance = 30;

        private IRepository repositoryContact;

        public ObservableCollection<Contact> Contacts;

        public ContactList()
        {
            InitializeComponent();
            
            Contacts = new ObservableCollection<Contact>();
            ContactsView.ItemsSource = Contacts;
            this.repositoryContact = new ContactRedoLog();
            RefreshContactList();
        }

        private void RefreshContactList()
        {
            var list = repositoryContact.getAllContacts();
            Contacts.Clear();
            foreach (Contact contact in list)
                Contacts.Add(contact);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            // selon le text label, appelle la méthode correspondante pour gérer appel ou sms
        }

        private void SMSClicked()
        {

        }

        private void TelClicked()
        {

        }
        
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Contact contact = ((StackLayout)sender).BindingContext as Contact;

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    HandleTouch((View)sender, (float)e.TotalX);
                    break;
                case GestureStatus.Completed:
                    HandleTouchEnd((View)sender, contact);
                    break;
            }
        }

        private async void HandleTouch(View sender, float diff_x)
        {
            await sender.TranslateTo(diff_x, 0);
            SwipeDistance = diff_x;
        }

        private async void HandleTouchEnd(View sender, Contact contact)
        {
            if (Math.Abs(SwipeDistance) > ReferenceSwipeDistance)
            {
                // envoyer l'élément hors du champ
                await sender.TranslateTo(SwipeDistance > 0 ? this.Width : -this.Width, 0);
                if (SwipeDistance > 0)
                {
                    await ResetElementPosition(sender);
                    SwipedRight(contact);
                }
                else
                {
                    SwipedLeft(contact).ContinueWith(
                        (a) => ResetElementPosition(sender)
                        );
                }
            }
            else
            {
                await ResetElementPosition(sender);
            }
        }

        private async Task ResetElementPosition(View element)
        {
            await element.TranslateTo(-element.X, 0);
            SwipeDistance = 0;
        }
        
        /// <summary>
        /// Supprime le contact
        /// </summary>
        private void SwipedRight(Contact contact)
        {
            this.repositoryContact.deleteContact(contact.Id);
            RefreshContactList();
        }

        /// <summary>
        /// Redirige vers la page de détail contact
        /// </summary>
        private Task SwipedLeft(Contact contact)
        {
            return this.Navigation.PushAsync(new ContactDetail(contact.Id));
        }

        //private EventHandler GetSMSEventHandler(string PhoneNumber)
        //{
        //    // les EventHandler doivent avoir pour paramètres (object sender, EventArgs e)
        //    // ici on renvoie une fonction de type EventHandler, qui appellera SMS_Clicked avec 
        //    // le bon paramètre PhoneNumber
        //    return (sender, e) => SMS_Clicked(PhoneNumber);
        //}

        //private void SMS_Clicked(string PhoneNumber)
        //{
        //    DependencyService.Get<ICellPhone>().openSMS(PhoneNumber);
        //}
    }
}
