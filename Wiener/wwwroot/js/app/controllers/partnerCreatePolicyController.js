var PartnerCreatePolicyController = (function (partnerService) {
    'use strict';

    var urlInfo = {};

    var initializeUrls = function (urls) {
        urlInfo = urls;
    };

    var showFormErrors = function (errors) {
        var errorHtml = '<ul class="mb-0">';
        errors.forEach(function (error) {
            errorHtml += '<li>' + error + '</li>';
        });
        errorHtml += '</ul>';

        $('#policyFormErrors').removeClass('d-none').html(errorHtml);
    };

    var showSuccessAndReload = function (message) {
        $('#policyModal').modal('hide');

        var alert = '<div class="alert alert-success alert-dismissible fade show">' +
            '<i class="bi bi-check-circle"></i> ' + message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>';

        $('.container-fluid').prepend(alert);

        setTimeout(function () {
            location.reload();
        }, 1000);
    };

    var resetSubmitButton = function () {
        $('#savePolicyBtn')
            .prop('disabled', false)
            .html('<i class="bi bi-save"></i> Spremi policu');
    };

    var handlePolicyFormSubmit = function (e) {
        e.preventDefault();

        $('#policyFormErrors').addClass('d-none').html('');
        $('#savePolicyBtn')
            .prop('disabled', true)
            .html('<span class="spinner-border spinner-border-sm"></span> Spremanje...');

        var formData = $('#createPolicyForm').serialize();

        partnerService.createPolicy(urlInfo.createPolicyUrl, formData)
            .done(function (response) {
                if (response.success) {
                    showSuccessAndReload(response.message);
                } else {
                    showFormErrors(response.errors);
                    resetSubmitButton();
                }
            })
            .fail(function () {
                showFormErrors(['Greška prilikom komunikacije sa serverom']);
                resetSubmitButton();
            });
    };

    var bindEvents = function () {
        $(document).on('submit', '#createPolicyForm', handlePolicyFormSubmit);
    };

    var init = function (urls) {
        initializeUrls(urls);
        bindEvents();
    };

    return {
        init: init
    };

})(PartnerService);