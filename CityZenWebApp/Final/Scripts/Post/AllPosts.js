//$(function () {
    
//})
function DeletePost(postId) {
    var res = confirm("Are you sure?");
    if (res) {
        $.ajax({
            type: 'post',
            url: '/Post/DeletePost',
            data: { postId: postId },
            success: function (data) {
                window.location.reload();
            },
            error: function (err) {
                window.location.reload();
            }
        });
    }
}

function ApplyFilters() {
    var category = $('#Categories').val();
    var city = $('#City').val();
    var address = $('#Street').val();
    $.ajax({
        type:'GET',
        url: "/Post/_Posts",
        data: { category: category, city: city, address: address },
        success: function (res) {
            $('#partial_view').html(res);
        },
        error: function (err) {
            console.log(err);
        }
    })
}
function ClearFilters() {
    $.ajax({
        url: "/Post/_Posts",
        success: function (res) {
            $('#partial_view').html(res);
        }
    })
}
function GetAddresses(city) {
    if (city != "--Select City--" && city != "") {
        $.ajax({
            url: '/Post/GetAddressesByCity',
            data: { city: city },
            success: function (data) {
                var ddl = $('#Street');
                ddl.empty();
                
                $(data).each(function () {
                    $(document.createElement('option'))
                    .attr('value', this)
                    .text(this)
                    .appendTo(ddl);
                })
                //$('#Street').html(data);
            }
        })
    }
}
function Upvote(postId) {
    $.ajax({
        type: 'POST',
        url: '/Post/UpvotePost',
        data: { postId: postId },
        success: function (data) {
            if (data == "ok") {
                var selector = "#" + postId;
                $('#upvoted').fadeIn(500).fadeOut(500);
                var up = parseInt($(selector).text());
                $(selector).text(up + 1);
            } else {
                alert(data);
            }
            
        },
        error: function (err) {
            alert(err);
        }
    })
}
function Downvote(postId) {
    $.ajax({
        type: 'post',
        url: '/Post/Downvote',
        data: { postId: postId },
        success: function (data) {
            if (data == "ok") {
                var selector = "#" + postId;
                var up = parseInt($(selector).text());
                $(selector).text(up - 1);
            } else {
                alert(data);
            }
        },
        error: function (er) {
            alert(err);
        }

    
    })
}