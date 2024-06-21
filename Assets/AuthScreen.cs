using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class AuthScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI screenTitleTxt;
    [SerializeField] TMP_InputField countryCodeInput,phoneInput,mailInput;
    [SerializeField] GameObject agreementContent;
    [SerializeField] Toggle agreement_Terms, agreement_2;
    [SerializeField] Button signUpsignInSwitchBt;
    [SerializeField] TextMeshProUGUI signUpsignInTxt, signUpsignInBtTxt;
    [SerializeField] Button sendOtpBt;
    bool signUpActive = true;
    private void Start()
    {
        signUpsignInSwitchBt.onClick.AddListener(SwitchSignUpSignIn);
        countryCodeInput.onValueChanged.AddListener(SetOtpBtInteractable);
        phoneInput.onValueChanged.AddListener(SetOtpBtInteractable);
        mailInput.onValueChanged.AddListener(SetOtpBtInteractable);
        agreement_Terms.onValueChanged.AddListener(SetOtpBtInteractable);
        agreement_2.onValueChanged.AddListener(SetOtpBtInteractable);

    }
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
}
