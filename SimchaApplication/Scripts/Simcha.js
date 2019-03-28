$(() => {

    $("#new-simcha").on('click', function () {

        $('#simchaModal').modal('show');
        $("#simcha-name").val('');
        $("#simcha-date").val('');
    })

    $("#new-contributor").on('click', function () {

        $('#ContributorModal').modal('show');
        $("#first-name").val('');
        $("#last-name").val('');
        $("#cell-number").val('');
        $("#diposit").val('');
        $("#simcha-date").val('');
        $("#Checkbox").val('');


    })

})