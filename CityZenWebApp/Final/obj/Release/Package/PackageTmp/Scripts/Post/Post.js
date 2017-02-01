
//$(function(){

function MarkPostResolved(option) {
    var pId = $('#postId').val();
    var postId = parseInt(pId);
    $.ajax({
        url: '/Post/MarkPostResolved',
        data: { postId: postId, appOrRes: option },
        type: 'get',
        success:function(data){
            window.location.reload();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(xhr.responseText);
            console.log(thrownError);
            window.location.reload();
        }
    })
}


function AddComment() {
    var text = $('#comment_text').val();
    var postId = $('#postId').val();
    $('.spinner').show();
    $.ajax({
        type: 'POST',
        url: '/Post/AddComment',
        data: { postId: postId, text: text },
        success: function (data) {
            $('#partial_comments').html(data);
            $('#success_msg').fadeIn(500).fadeOut(500);
            $('#comment_text').val("");
            $('.spinner').hide();
        },
        error: function (err) {
            console.log(err);
            $('.spinner').hide();
        }
    })
}
//})

