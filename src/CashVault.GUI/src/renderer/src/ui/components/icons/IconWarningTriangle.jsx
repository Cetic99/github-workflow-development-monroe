/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconWarningTriangle = ({ color = 'black', size = 'm' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M18.7824 16.1072L13.7896 6.1039C13.053 4.62814 10.9472 4.62812 10.2106 6.10389L5.21763 16.1071C4.55392 17.4369 5.52095 19.0003 7.0071 19.0003H16.9929C18.479 19.0003 19.4461 17.4369 18.7824 16.1072ZM15.579 5.21073C14.1059 2.2592 9.89432 2.25918 8.42112 5.2107L3.42816 15.2139C2.10074 17.8734 4.03478 21.0003 7.0071 21.0003H16.9929C19.9652 21.0003 21.8992 17.8734 20.5719 15.214L15.579 5.21073Z"
          fill="currentColor"
        />
        <path
          d="M13 8C13 7.44772 12.5523 7 12 7C11.4477 7 11 7.44772 11 8V12C11 12.5523 11.4477 13 12 13C12.5523 13 13 12.5523 13 12V8Z"
          fill="currentColor"
        />
        <path
          d="M12 15C11.4477 15 11 15.4477 11 16C11 16.5523 11.4477 17 12 17C12.5523 17 13 16.5523 13 16C13 15.4477 12.5523 15 12 15Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconWarningTriangle
