using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

public class AuthScreen : MonoBehaviour
{
    [Header("SignUp SignIn")]
    [SerializeField] GameObject SignUpSingInScreen;
    [SerializeField] TextMeshProUGUI screenTitleTxt;
    [SerializeField] TMP_InputField countryCodeInput,phoneInput,mailInput;
    [SerializeField] GameObject agreementContent;
    [SerializeField] Toggle agreement_Terms, agreement_2;
    [SerializeField] Button signUpsignInSwitchBt;
    [SerializeField] TextMeshProUGUI signUpsignInTxt, signUpsignInBtTxt;
    [SerializeField] Button sendOtpBt;

    [Header("OTP Screen")]
    [SerializeField] GameObject OTPScreen;
    [SerializeField] TextMeshProUGUI otpEmailTxt;
    [SerializeField] TMP_InputField otpInput;
    [SerializeField] Button editMailPhoneInput;
    [SerializeField] Button verifyOTP;
    [SerializeField] Button resendOTP;

    UserMailAndPhone_JStruct userMailAndPhone_JStruct;
    UserOTP_JStruct userOTP_JStruct;
    bool signUpActive = true;
    private void Start()
    {
        signUpsignInSwitchBt.onClick.AddListener(SwitchSignUpSignIn);
        countryCodeInput.onValueChanged.AddListener(SetOtpBtInteractable);
        phoneInput.onValueChanged.AddListener(SetOtpBtInteractable);
        mailInput.onValueChanged.AddListener(SetOtpBtInteractable);
        agreement_Terms.onValueChanged.AddListener(SetOtpBtInteractable);
        agreement_2.onValueChanged.AddListener(SetOtpBtInteractable);
        sendOtpBt.onClick.AddListener(SendUserPhoneAndMail);

        otpInput.onValueChanged.AddListener(SetVerirfyOtpBtInteractable);
        editMailPhoneInput.onClick.AddListener(OpenSignUpSignInScreen);
        resendOTP.onClick.AddListener(SendUserPhoneAndMail);
        verifyOTP.onClick.AddListener(SendVerifyOTP);
    }

    #region Auth
    void SendUserPhoneAndMail()
    {
        if (userMailAndPhone_JStruct == null)
            userMailAndPhone_JStruct = new UserMailAndPhone_JStruct();
        userMailAndPhone_JStruct.PhoneNumber = phoneInput.text;
        userMailAndPhone_JStruct.EmailID = mailInput.text;
        APIHandler.instance.PostUserMailPhone(userMailAndPhone_JStruct,AuthCallback);
    }
    void AuthCallback(bool success, UserData_JStruct userData_JStruct)
    {
        if(success && userData_JStruct.Status)        
            OpenOTPScreen();        
    }
    #endregion

    #region OTP
    void SendVerifyOTP()
    {
        if (userOTP_JStruct == null)
            userOTP_JStruct = new UserOTP_JStruct();
        userOTP_JStruct.PhoneNumber = phoneInput.text;
        userOTP_JStruct.EmailID = mailInput.text;
        userOTP_JStruct.OTP = otpInput.text;
        APIHandler.instance.PostUserAuthOTP(userOTP_JStruct, OtpCallback);
    }
    void OtpCallback(bool success, UserData_JStruct userData_JStruct)
    {
        if (success && userData_JStruct.Status)
        {
            //redirect to game
        }
    }
    #endregion

    #region validation
    void SwitchSignUpSignIn()
    {
        signUpActive = !signUpActive;
        screenTitleTxt.text = signUpActive ? "Sign Up" : "Sign In";
        signUpsignInTxt.text = signUpActive ? "Existing User?" : "New User?";
        signUpsignInBtTxt.text = signUpActive ? "Sign in here." : "Sign up here.";
        agreementContent.SetActive(signUpActive);
    }
    void SetOtpBtInteractable(bool bl)
    {
        SetOtpBtInteractable();
    }
    void SetOtpBtInteractable(string str)
    {
        SetOtpBtInteractable();
    }
    void SetOtpBtInteractable()
    {
        sendOtpBt.interactable = CheckFields();
    }
    bool CheckFields()
    {
        bool countryCode = countryCodeInput.text.Trim() != "";
        bool phone = phoneInput.text.Trim().Length >= 10;
        bool mail = ValidateEmail(mailInput.text);
        bool agreement = agreement_Terms.isOn && agreement_2.isOn;
        if (signUpActive)
            return countryCode && phone && mail && agreement;
        else
            return countryCode && phone && mail;
    }

    void SetVerirfyOtpBtInteractable(string str)
    {
        verifyOTP.interactable = CheckOTPInput();
    }
    bool CheckOTPInput()
    {
        otpInput.text.Trim();
        if (otpInput.text.Length == 6)
        {
            return true;
        }
        else
        {
            if (otpInput.text.Length > 6)
            {
                for (int i = 6; i < otpInput.text.Length - 1; i++)
                {
                    otpInput.text.Remove(i);
                }
            }
            return false;
        }
    }

    public const string EmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@" +
        @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\." +
        @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|" +
        @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$"; 

    public static bool ValidateEmail(string email)
    {
        if (!string.IsNullOrEmpty(email))
            return Regex.IsMatch(email, EmailPattern);
        else
            return false;
    }
    #endregion

    #region Screens handling
    void OpenSignUpSignInScreen()
    {
        SignUpSingInScreen.SetActive(true);
        OTPScreen.SetActive(false);
    }
    void OpenOTPScreen()
    {
        SignUpSingInScreen.SetActive(false);
        OTPScreen.SetActive(true);
        otpEmailTxt.text = mailInput.text;
    }
    #endregion
}
