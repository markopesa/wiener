var PartnerIndexController = (function (partnerService) {
    'use strict';

    var urlInfo = {};

    var initializeUrls = function (urls) {
        urlInfo = urls;
    };

    var handlePartnerRowClick = function () {
        var partnerId = $(this).data('partner-id');

        partnerService.getPartnerDetails(
            urlInfo.partnerDetailsUrl,
            partnerId
        )
            .done(function (data) {
                $('#partnerDetailsContent').html(data);
                $('#partnerDetailsModal').modal('show');
            })
            .fail(function () {
                console.error('Greška prilikom dohvaćanja detalja partnera');
            });
    };

    var handleAddPolicyClick = function (e) {
        e.stopPropagation();
        var partnerId = $(this).data('partner-id');

        partnerService.loadPolicyModal(
            urlInfo.createPolicyModalUrl,
            partnerId
        )
            .done(function (data) {
                $('#policyModalContent').html(data);
                $('#policyModal').modal('show');
            })
            .fail(function () {
                console.error('Greška prilikom učitavanja forme za policu');
            });
    };

    var bindEvents = function () {
        $('.partner-row').on('click', handlePartnerRowClick);
        $('.add-policy-btn').on('click', handleAddPolicyClick);
    };

    var init = function (urls) {
        initializeUrls(urls);
        bindEvents();
    };

    return {
        init: init
    };

})(PartnerService);