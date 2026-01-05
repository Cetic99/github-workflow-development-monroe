/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

const images = import.meta.glob('@ui/assets/images/parcel-locker/*.png', {
  eager: true,
  import: 'default'
})

const PostalServiceImage = ({ code, className = 'postal-service-image', alt = code }) => {
  const imagePath = `/src/ui/assets/images/parcel-locker/${code}.png`
  const src = images[imagePath]
  const fallbackSrc = `/src/ui/assets/images/parcel-locker/postal-service-fallback.png`

  if (!src) {
    return <img className={className} src={fallbackSrc} alt={alt} />
  }

  return <img className={className} src={src} alt={alt} />
}
export default PostalServiceImage
