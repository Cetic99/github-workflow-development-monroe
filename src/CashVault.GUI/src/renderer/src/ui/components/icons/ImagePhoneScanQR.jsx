import styled from '@emotion/styled'
import Image from '@ui/assets/images/parcel-locker/scan-phone.png'

const Container = styled.div`
  position: relative;
  width: fit-content;
  width: 600px;
  height: 600px;

  & svg {
    z-index: 3;
    position: absolute;
    top: 0;
    left: 0;
  }

  & img {
    z-index: 2;
    position: absolute;
    top: 0;
    left: 0;

    width: 600px;
    height: 600px;
  }
`

const ImagePhoneScanQR = () => {
  return (
    <Container className="image-phone-scan-qr">
      <svg
        width="600"
        height="600"
        viewBox="0 0 600 600"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        xmlnsXlink="http://www.w3.org/1999/xlink"
      >
        <rect width="600" height="600" fill="url(#pattern0_930_3350)" />
        <rect
          x="253"
          y="102"
          width="93"
          height="18"
          rx="5"
          fill="#004B3E"
          stroke="black"
          strokeWidth="2"
        />
        <rect x="258" y="107" width="83" height="8" rx="2" fill="url(#paint0_linear_930_3350)" />
        <path d="M299.5 135L466.21 290.25H132.79L299.5 135Z" fill="url(#paint1_linear_930_3350)" />
        <defs>
          <pattern
            id="pattern0_930_3350"
            patternContentUnits="objectBoundingBox"
            width="1"
            height="1"
          >
            <use xlinkHref="#image0_930_3350" transform="scale(0.0005)" />
          </pattern>
          <linearGradient
            id="paint0_linear_930_3350"
            x1="299.5"
            y1="115"
            x2="299.5"
            y2="107"
            gradientUnits="userSpaceOnUse"
          >
            <stop stopColor="#004B3E" />
            <stop offset="1" stopColor="#00B192" />
          </linearGradient>
          <linearGradient
            id="paint1_linear_930_3350"
            x1="299.5"
            y1="135"
            x2="299.5"
            y2="342"
            gradientUnits="userSpaceOnUse"
          >
            <stop stopColor="#2BD6B5" stopOpacity="0.75" />
            <stop offset="1" stopColor="#2BD6B5" stopOpacity="0" />
          </linearGradient>
        </defs>
      </svg>

      <img src={Image} />
    </Container>
  )
}

export default ImagePhoneScanQR
