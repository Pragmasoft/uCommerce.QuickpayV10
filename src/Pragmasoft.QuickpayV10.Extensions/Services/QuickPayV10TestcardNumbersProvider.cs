using System;
using System.Collections.Generic;
using Pragmasoft.QuickpayV10.Extensions.Services.Interfaces;

namespace Pragmasoft.QuickpayV10.Extensions.Services
{

    public class QuickPayV10TestcardNumbersProvider : IQuickPayV10TestcardNumbersProvider
    {

        private IEnumerable<String> _testCards;

        public IEnumerable<String> GetCardNumbers()
        {
            return _testCards ?? (_testCards = new []
            {
                "1000 0000 0000 0008", // VISA - Approved
                "1000 0000 0000 0016", // VISA - Rejected
                "1000 0000 0000 0024", // VISA - Card Expired
                "1000 0000 0000 0032", // VISA - Capture Rejected
                "1000 0000 0000 0040", // VISA - Refund Rejected
                "1000 0000 0000 0057", // VISA - Cancel Rejected
                "1000 0000 0000 0065", // VISA - Recurring Rejected
                "1000 0000 0000 0073", // VISA - 30100 - 3D Secure is required 
                "1000 0100 0000 0007", // Mastercard - Approved
                "1000 0100 0000 0015", // Mastercard - Rejected
                "1000 0100 0000 0023", // Mastercard - Card Expired
                "1000 0100 0000 0031", // Mastercard - Capture Rejected
                "1000 0100 0000 0049", // Mastercard - Refund Rejected
                "1000 0100 0000 0056", // Mastercard - Cancel Rejected
                "1000 0100 0000 0064", // Mastercard - Recurring Rejected
                "1000 0100 0000 0072", // Mastercard - 30100 - 3D Secure is required 
                "1000 0200 0000 0006", // Dankort - Approved
                "1000 0200 0000 0014", // Dankort - Rejected
                "1000 0200 0000 0022", // Dankort - Card Expired
                "1000 0200 0000 0030", // Dankort - Capture Rejected
                "1000 0200 0000 0048", // Dankort - Refund Rejected
                "1000 0200 0000 0055", // Dankort - Cancel Rejected
                "1000 0200 0000 0063", // Dankort - Recurring Rejected
                "1000 0300 0000 0005", // American Express - Approved
                "1000 0300 0000 0013", // American Express - Rejected
                "1000 0300 0000 0021", // American Express - Card Expired
                "1000 0300 0000 0039", // American Express - Capture Rejected
                "1000 0300 0000 0047", // American Express - Refund Rejected
                "1000 0300 0000 0054", // American Express - Cancel Rejected
                "1000 0300 0000 0062", // American Express - Recurring Rejected
                "1000 0400 0000 0004", // Maestro - Approved
                "1000 0400 0000 0012", // Maestro - Rejected
                "1000 0400 0000 0020", // Maestro - Card Expired
                "1000 0400 0000 0038", // Maestro - Capture Rejected
                "1000 0400 0000 0046", // Maestro - Refund Rejected
                "1000 0400 0000 0053", // Maestro - Cancel Rejected
                "1000 0400 0000 0061", // Maestro - Recurring Rejected
                "1000 0500 0000 0003", // VISA Electron - Approved
                "1000 0500 0000 0011", // VISA Electron - Rejected
                "1000 0500 0000 0029", // VISA Electron - Card Expired
                "1000 0500 0000 0037", // VISA Electron - Capture Rejected
                "1000 0500 0000 0045", // VISA Electron - Refund Rejected
                "1000 0500 0000 0052", // VISA Electron - Cancel Rejected
                "1000 0500 0000 0060", // VISA Electron - Recurring Rejected
                "1000 0500 0000 0078", // VISA Electron - 30100 - 3D Secure is required 
                "1000 0600 0000 0002", // VISA/Dankort - Approved
                "1000 0600 0000 0010", // VISA/Dankort - Rejected
                "1000 0600 0000 0028", // VISA/Dankort - Card Expired
                "1000 0600 0000 0036", // VISA/Dankort - Capture Rejected
                "1000 0600 0000 0044", // VISA/Dankort - Refund Rejected
                "1000 0600 0000 0051", // VISA/Dankort - Cancel Rejected
                "1000 0600 0000 0069", // VISA/Dankort - Recurring Rejected
                "1000 0600 0000 0077", // VISA/Dankort - 30100 - 3D Secure is required 
                "1000 0700 0000 0001", // Diners - Approved
                "1000 0700 0000 0019", // Diners - Rejected
                "1000 0700 0000 0027", // Diners - Card Expired
                "1000 0700 0000 0035", // Diners - Capture Rejected
                "1000 0700 0000 0043", // Diners - Refund Rejected
                "1000 0700 0000 0050", // Diners - Cancel Rejected
                "1000 0700 0000 0068", // Diners - Recurring Rejected
                "1000 0800 0000 0000", // FBG1886 - Approved
                "1000 0800 0000 0018", // FBG1886 - Rejected
                "1000 0800 0000 0026", // FBG1886 - Card Expired
                "1000 0800 0000 0034", // FBG1886 - Capture Rejected
                "1000 0800 0000 0042", // FBG1886 - Refund Rejected
                "1000 0800 0000 0059", // FBG1886 - Cancel Rejected
                "1000 0800 0000 0067", // FBG1886 - Recurring Rejected)
            });
        } 
    }
}
