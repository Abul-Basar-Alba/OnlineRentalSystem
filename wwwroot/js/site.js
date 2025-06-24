$(document).ready(function () {
    $('input[type="date"]').each(function () {
        $(this).attr('min', new Date().toISOString().split('T')[0]);
    });

    $('#StartDate, #EndDate, #ListingId').change(function () {
        var startDate = new Date($('#StartDate').val());
        var endDate = new Date($('#EndDate').val());
        var listingId = $('#ListingId').val();
        if (startDate && endDate && endDate > startDate && listingId) {
            $.get('/Listings/GetPrice/' + listingId, function (data) {
                var days = (endDate - startDate) / (1000 * 60 * 60 * 24);
                $('#TotalPrice').val((days * data.pricePerDay).toFixed(2));
            });
        }
    });
});