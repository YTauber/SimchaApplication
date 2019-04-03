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

    $(".diposit").on('click', function () {

        $('#DipositModal').modal('show');
        $("#amount").val('');
        $("#diposit-date").val('');
        let x = $(this).data('id');
        $("#id").val(x);
    })

    $(".edit").on('click', function () {

        $('#EditModal').modal('show');
        $("#first-edit").val($(this).data('first'));
        $("#last-edit").val($(this).data('last'));
        $("#cell-edit").val($(this).data('cell'));
        $("#date-edit").val($(this).data('date'));
        
        $("#Checkbox-edit").prop('checked', $(this).data('alw') == true);
      
        let y = $(this).data('edit-id');
        $("#edit-id").val(y);
    })

})