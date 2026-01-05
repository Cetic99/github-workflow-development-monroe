/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconRepeat = ({ color = 'black', size = 'm' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="32"
        height="32"
        viewBox="0 0 32 32"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M6.66634 16C7.40272 16 7.99967 16.597 7.99967 17.3333V20C7.99967 21.4728 9.19358 22.6667 10.6663 22.6667H23.9997C24.7361 22.6667 25.333 23.2636 25.333 24C25.333 24.7364 24.7361 25.3333 23.9997 25.3333H10.6663C7.72082 25.3333 5.33301 22.9455 5.33301 20V17.3333C5.33301 16.597 5.92996 16 6.66634 16Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M24.9431 23.0571C25.4638 23.5778 25.4638 24.4221 24.9431 24.9428L20.9431 28.9428C20.4224 29.4635 19.5782 29.4635 19.0575 28.9428C18.5368 28.4221 18.5368 27.5779 19.0575 27.0572L22.1147 24L19.0575 20.9428C18.5368 20.4221 18.5368 19.5778 19.0575 19.0572C19.5782 18.5365 20.4224 18.5365 20.9431 19.0572L24.9431 23.0571Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M6.66699 7.99996C6.66699 7.26358 7.26395 6.66663 8.00033 6.66663H21.3337C24.2792 6.66663 26.667 9.05444 26.667 12V14.6666C26.667 15.403 26.07 16 25.3337 16C24.5973 16 24.0003 15.403 24.0003 14.6666V12C24.0003 10.5272 22.8064 9.33329 21.3337 9.33329H8.00033C7.26395 9.33329 6.66699 8.73634 6.66699 7.99996Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M7.05752 8.94277C6.53682 8.42207 6.53682 7.57785 7.05752 7.05715L11.0575 3.05715C11.5782 2.53645 12.4224 2.53645 12.9431 3.05715C13.4638 3.57785 13.4638 4.42207 12.9431 4.94277L9.88594 7.99996L12.9431 11.0571C13.4638 11.5778 13.4638 12.4221 12.9431 12.9428C12.4224 13.4635 11.5782 13.4635 11.0575 12.9428L7.05752 8.94277Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconRepeat
