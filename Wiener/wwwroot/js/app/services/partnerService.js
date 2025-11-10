var PartnerService = (function () {
    'use strict';

    var getPartnerDetails = function (url, partnerId) {
        return $.ajax({
            url: url,
            type: 'GET',
            data: { id: partnerId }
        });
    };

    var createPolicy = function (url, formData) {
        return $.ajax({
            url: url,
            type: 'POST',
            data: formData
        });
    };

    var loadPolicyModal = function (url, partnerId) {
        return $.ajax({
            url: url,
            type: 'GET',
            data: { partnerId: partnerId }
        });
    };

    return {
        getPartnerDetails: getPartnerDetails,
        createPolicy: createPolicy,
        loadPolicyModal: loadPolicyModal
    };

})();