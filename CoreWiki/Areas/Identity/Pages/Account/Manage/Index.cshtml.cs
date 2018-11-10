﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CoreWiki.Data.EntityFramework.Security;
using CoreWiki.Notifications.Abstractions.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreWiki.Areas.Identity.Pages.Account.Manage
{
	public partial class IndexModel : PageModel
	{
		private readonly UserManager<CoreWikiUser> _userManager;
		private readonly SignInManager<CoreWikiUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly INotificationService _notificationService;

		public IndexModel(
					UserManager<CoreWikiUser> userManager,
					SignInManager<CoreWikiUser> signInManager,
					IEmailSender emailSender,
					INotificationService notificationService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_notificationService = notificationService;
		}

		public string Username { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			public string Email { get; set; }

			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }

			[Display(Name = "Opt-in to receive notifications?")]
			public bool CanNotify { get; set; }

			[Required]
			[Display(Name ="Name to display to other users")]
			public string DisplayName { get; set; }
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var userName = await _userManager.GetUserNameAsync(user);
			var email = await _userManager.GetEmailAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

			Username = userName;

			Input = new InputModel
			{
				DisplayName = user.DisplayName,
				Email = email,
				PhoneNumber = phoneNumber,
				CanNotify = user.CanNotify
			};

			IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var email = await _userManager.GetEmailAsync(user);
			if (Input.Email != email)
			{
				var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
				if (!setEmailResult.Succeeded)
				{
					var userId = await _userManager.GetUserIdAsync(user);
					throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
				}
			}

			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			if (Input.PhoneNumber != phoneNumber)
			{
				var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
				if (!setPhoneResult.Succeeded)
				{
					var userId = await _userManager.GetUserIdAsync(user);
					throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
				}
			}

			if (Input.CanNotify != user.CanNotify)
			{
				user.CanNotify = Input.CanNotify;
				var updateProfileResult = await _userManager.UpdateAsync(user);

				if (!updateProfileResult.Succeeded)
				{
					throw new InvalidOperationException($"Unexpected error ocurred updating CanNotify for user with ID '{user.Id}'");
				}
			}

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostSendVerificationEmailAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}


			var userId = await _userManager.GetUserIdAsync(user);
			var email = await _userManager.GetEmailAsync(user);
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			await _notificationService.SendConfirmationEmail(email, userId, code);

			StatusMessage = "Verification email sent. Please check your email.";
			return RedirectToPage();
		}
	}
}
