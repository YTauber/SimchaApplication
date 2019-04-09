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
        $("#simcha-date").val($("#simcha-date").data('date'));
        $("#Checkbox").prop("checked", false);


    })

    $(".diposit").on('click', function () {

        $('#DipositModal').modal('show');
        $("#amount").val('');
        $("#diposit-date").val($("#diposit-date").data('date'));
        let x = $(this).data('id');
        $("#id").val(x);
        $(".modal-title").text(`Diposit for ${$(this).data('name')}`)
    })

    $(".edit").on('click', function () {

        $('#EditModal').modal('show');
        $("#first-edit").val($(this).data('first'));
        $("#last-edit").val($(this).data('last'));
        $("#cell-edit").val($(this).data('cell'));
        $("#date-edit").val($(this).data('date'));
        
        $("#Checkbox-edit").prop('checked', $(this).data('alw') === "True");
      
        let y = $(this).data('edit-id');
        $("#edit-id").val(y);
    })

    $("#simchaModal").on('change', function () {
        
        $("#simcha-submit").prop('disabled', !$("#simcha-name").val() || !$("#simcha-date").val())
    })

    $("#ContributorModal").on('change', function () {

        $("#cont-submit").prop('disabled', !$("#first-name").val() || !$("#last-name").val() || !$("#cell-number").val() || !$("#diposit").val() || !$("#simcha-date").val())
    })

    $("#DipositModal").on('change', function () {

        $("#diposit-submit").prop('disabled', !$("#amount").val() || !$("#diposit-date").val())
    })

    $("#EditModal").on('change', function () {

        $("#edit-submit").prop('disabled', !$("#first-edit").val() || !$("#last-edit").val() || !$("#cell-edit").val() || !$("#date-edit").val())
    })

    $(".search").on("keyup", show);

    $(".clear").on('click', function () {

        $(".search").val('');
        show();
    })

    function show() {

        var value = $(".search").val().toLowerCase();
        $(".cont-table tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    }

})