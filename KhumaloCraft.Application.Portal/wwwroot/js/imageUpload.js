$(document).ready(function () {
    $('input[type="file"].image-upload').on('change', function () {
        const file = this.files[0];

        const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];

        if (file && validTypes.includes(file.type)) {
            const formData = new FormData();
            formData.append('file', file);

            $.ajax({
                url: '/Images/UploadImage', // URL to upload endpoint
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    const previewContainer = $(this).data('preview-container');
                    const hiddenInput = $(this).data('hidden-input');

                    $(hiddenInput).val(response.id);

                    if (previewContainer) {
                        $(previewContainer).html('<img src="' + response.thumbnail + '" alt="Image Preview" style="max-width: 200px;"/>');
                    }
                }.bind(this), // Bind 'this' to access the input element
                error: function (error) {
                    console.error('Error uploading image:', error);
                }
            });
        } else {
            alert('Please select a valid image file (JPEG or PNG).');
            $(this).val('');
        }
    });
});